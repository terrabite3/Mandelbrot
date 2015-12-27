using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class Tile : MonoBehaviour
{
    

    public int level;
    public string address;
    public double centerX;
    public double centerY;
    public int iterations;
    public TileManager Manager;

    private Thread renderThread = null;

    private static int threadCount = 0;

    public void Init(TileManager man, int v1, string addr, double v2, double v3)
    {
        Manager = man;
        level = v1;
        address = addr;
        centerX = v2;
        centerY = v3;
        splittable = true;

        gameObject.name = "Tile " + HexAddress;

        iterations = (int)CutoffManager.cutoff + 100;



        if (threadCount > 6)
        {
            Debug.Log("Switching to GPU");

            RenderTexture texture = new RenderTexture(Manager.textureSize, Manager.textureSize, 32, RenderTextureFormat.RFloat);
            texture.enableRandomWrite = true;
            texture.Create();

            Manager.shader.SetInt("textureSize", Manager.textureSize);
            Manager.shader.SetInt("maxIt", iterations);

            if (level < 17) // Magic number: around zoom 2^17, the float is not enough precision
            {
                Manager.shader.SetTexture(Manager.floatKernel, "Result", texture);
                Manager.shader.SetFloats("bounds", new float[] { (float)Top, (float)Left, (float)Bottom, (float)Right });

                Manager.shader.Dispatch(Manager.floatKernel, Manager.textureSize / 16, Manager.textureSize / 16, 1);
            }
            else
            {
                Manager.shader.SetTexture(Manager.doubleKernel, "Result", texture);

                Manager.shader.SetInts("leftInts", DoubleAsInts(Left));
                Manager.shader.SetInts("rightInts", DoubleAsInts(Right));
                Manager.shader.SetInts("topInts", DoubleAsInts(Top));
                Manager.shader.SetInts("bottomInts", DoubleAsInts(Bottom));

                Manager.shader.Dispatch(Manager.doubleKernel, Manager.textureSize / 4, Manager.textureSize / 4, 1);
            }

            gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
            texture.filterMode = FilterMode.Point;
        }
        else
        {

            GetComponent<MeshRenderer>().enabled = false;

            renderThread = new Thread(renderCPU);
            threadCount++;
            Debug.Log("Adding CPU thread #" + threadCount);
            renderThread.Start();
        }

        Update();


    }

    private float[,] renderData = null;

    private void renderCPU()
    {
        float[,] array = new float[Manager.textureSize, Manager.textureSize];

        for (int i = 0; i < Manager.textureSize; i++)
        {
            for (int j = 0; j < Manager.textureSize; j++)
            {
                double x0 = Left + (1.0 * i / Manager.textureSize) * (Right - Left);
                double y0 = Top + (1.0 * j / Manager.textureSize) * (Bottom - Top);

                double x = 0;
                double y = 0;

                double xSqr, ySqr, xTemp;

                int it = 0;

                while (true)
                {
                    xSqr = x * x;
                    ySqr = y * y;
                    if (xSqr + ySqr > 4.0) break;
                    if (it > iterations) break;

                    xTemp = xSqr - ySqr + x0;
                    y = 2.0 * x * y + y0;
                    x = xTemp;

                    it++;
                }

                if (it == iterations)
                    array[j, i] = -1f;
                else
                    array[j, i] = (float)(it - Math.Log(Math.Log(Math.Sqrt(xSqr + ySqr),2), 2));
            }
        }

        renderData = array;
    }


    public void Update()
    {
        if (renderData != null)
        {
            loadRenderedTexture();
        }

        double camZoom = CameraControl.zoom;
        double camX = CameraControl.centerX;
        double camY = CameraControl.centerY;

        float scale = (float)(2 * camZoom / Zoom);
        gameObject.transform.position = new Vector3(
            (float)((centerX - camX) * camZoom),
            (float)((centerY - camY) * camZoom),
            level / 10.0f);
        gameObject.transform.localScale = new Vector3(scale, scale, level / 10.0f);

    }

    private void loadRenderedTexture()
    {
        Debug.Log("Removing CPU thread #" + threadCount);
        threadCount--;

        byte[] byteArray = new byte[Manager.textureSize * Manager.textureSize * 4];
        for (int i = 0; i < Manager.textureSize; i++)
        {
            for (int j = 0; j < Manager.textureSize; j++)
            {
                byte[] floatAsBytes = BitConverter.GetBytes(renderData[i, j]);

                int index = (i * Manager.textureSize + j) * 4;

                byteArray[index + 0] = floatAsBytes[0];
                byteArray[index + 1] = floatAsBytes[1];
                byteArray[index + 2] = floatAsBytes[2];
                byteArray[index + 3] = floatAsBytes[3];
            }
        }

        Texture2D texture = new Texture2D(Manager.textureSize, Manager.textureSize, TextureFormat.RFloat, false);
        texture.LoadRawTextureData(byteArray);
        texture.Apply();
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        texture.filterMode = FilterMode.Point;

        renderData = null;
        GetComponent<MeshRenderer>().enabled = true;
    }

    private static int[] DoubleAsInts(double value)
    {
        long lValue = BitConverter.DoubleToInt64Bits(value);
        return new int[] {
                (int) (lValue & 0xFFFFFFFF),
                (int) ((lValue >> 32) & 0xFFFFFFFF)
            };
    }

    public double Zoom { get { return Math.Pow(2, level); } }

    public double Left { get { return centerX - 1.0 / Zoom; } }
    public double Right { get { return centerX + 1.0 / Zoom; } }
    public double Top { get { return centerY - 1.0 / Zoom; } }
    public double Bottom { get { return centerY + 1.0 / Zoom; } }

    public bool splittable = true;

    public Tile buildChild(int x, int y)
    {
        double cx, cy;
        switch (x)
        {
            case 0:
                cx = (Left + centerX) / 2;
                break;
            case 1:
                cx = (Right + centerX) / 2;
                break;
            default:
                throw new Exception("X should be 0 or 1");
        }
        switch (y)
        {
            case 0:
                cy = (Bottom + centerY) / 2;
                break;
            case 1:
                cy = (Top + centerY) / 2;
                break;
            default:
                throw new Exception("Y should be 0 or 1");
        }

        int addressNext = x + 2 * y;

        var result = Instantiate<GameObject>(gameObject);
        result.GetComponent<Tile>().Init(Manager, level + 1, address + addressNext, cx, cy);
        return result.GetComponent<Tile>();
    }

    public void RenderDeeper()
    {
        RenderTexture tex = gameObject.GetComponent<MeshRenderer>().material.GetTexture("_MainTex") as RenderTexture;

        if (tex == null)
            throw new Exception("Could not cast the texture as a RenderTexture!");

        Manager.shader.SetInt("textureSize", Manager.textureSize);
    }

    internal void Destroy()
    {
        GameObject.Destroy(gameObject, 0);
    }

    public Boolean IsInView()
    {
        Rect camRect = CameraControl.CameraRect;
        return (camRect.xMin <= Right) &&
            (camRect.xMax >= Left) &&
            (camRect.yMin <= Bottom) &&
            (camRect.yMax >= Top);
    }

    public string HexAddress
    {
        get
        {
            string result = "0x";
            for (int i = 0; i < address.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result += address[i];
                }
                else
                {
                    int number = Int32.Parse("" + address[i - 1]) * 4;
                    number += Int32.Parse("" + address[i]);
                    result = result.Substring(0, result.Length - 1);
                    result += number.ToString("X");
                }
            }

            return result;
        }
    }
}
