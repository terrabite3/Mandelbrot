using UnityEngine;
using System.Collections;
using System;

public class Tile : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePosition();
    }
    /*
    public int level;
    public double centerX;
    public double centerY;
    private GameObject plane;
    public int iterations;

    public Tile(int v1, double v2, double v3)
    {
        level = v1;
        centerX = v2;
        centerY = v3;

        iterations = (int)CutoffManager.cutoff + 100;

        RenderTexture texture = new RenderTexture(TileManager.inst.textureSize, TileManager.inst.textureSize, 32, RenderTextureFormat.RFloat);
        texture.enableRandomWrite = true;
        texture.Create();

        TileManager.inst.shader.SetInt("textureSize", TileManager.inst.textureSize);
        TileManager.inst.shader.SetInt("maxIt", iterations);

        if (level < 17) // Magic number: around zoom 2^17, the float is not enough precision
        {
            TileManager.inst.shader.SetTexture(TileManager.inst.floatKernel, "Result", texture);
            TileManager.inst.shader.SetFloats("bounds", new float[] { (float)Top, (float)Left, (float)Bottom, (float)Right });

            TileManager.inst.shader.Dispatch(TileManager.inst.floatKernel, TileManager.inst.textureSize / 16, TileManager.inst.textureSize / 16, 1);
        }
        else
        {
            TileManager.inst.shader.SetTexture(TileManager.inst.doubleKernel, "Result", texture);

            TileManager.inst.shader.SetInts("leftInts", DoubleAsInts(Left));
            TileManager.inst.shader.SetInts("rightInts", DoubleAsInts(Right));
            TileManager.inst.shader.SetInts("topInts", DoubleAsInts(Top));
            TileManager.inst.shader.SetInts("bottomInts", DoubleAsInts(Bottom));

            TileManager.inst.shader.Dispatch(TileManager.inst.doubleKernel, TileManager.inst.textureSize / 4, TileManager.inst.textureSize / 4, 1);
        }

        plane = (GameObject)Instantiate(TileManager.inst.tilePrefab);
        UpdatePosition();

        plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    }



    public void UpdatePosition()
    {
        // By scaling the tiles and zooming the camera each at sqrt(zoom),
        // we square the maximum zoom using floats
        //double camZoom = Math.Sqrt(CameraControl.zoom);
        double camZoom = CameraControl.zoom;
        double camX = CameraControl.centerX;
        double camY = CameraControl.centerY;

        float scale = (float)(2 * camZoom / Zoom);
        plane.transform.position = new Vector3(
            (float)((centerX - camX) * camZoom),
            (float)((centerY - camY) * camZoom),
            level / 10.0f);
        plane.transform.localScale = new Vector3(scale, scale, scale);

        //GetComponent<>
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
        return new Tile(level + 1, cx, cy);
    }

    public void RenderDeeper()
    {
        RenderTexture tex = plane.GetComponent<MeshRenderer>().material.GetTexture("_MainTex") as RenderTexture;

        if (tex == null)
            throw new Exception("Could not cast the texture as a RenderTexture!");

        TileManager.inst.shader.SetInt("textureSize", TileManager.inst.textureSize);
    }

    internal void Destroy()
    {
        GameObject.Destroy(plane, 0);
    }

    public Boolean IsInView()
    {
        Rect camRect = CameraControl.CameraRect;
        return (camRect.xMin <= Right) &&
            (camRect.xMax >= Left) &&
            (camRect.yMin <= Bottom) &&
            (camRect.yMax >= Top);
    }
    */
}
