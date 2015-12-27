using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileManager : MonoBehaviour {

    public static TileManager inst;

    public ComputeShader shader;
    public GameObject tilePrefab;
    private int floatKernel, doubleKernel;
    public int textureSize = 512;
    public float zoomFudge = 1.5f;

    private List<Tile> tiles = new List<Tile>();
    
	void Start () {
        if (inst != null)
            throw new Exception("More than one TileManager!");
        else
            inst = this;

        floatKernel = shader.FindKernel("FloatMain");
        doubleKernel = shader.FindKernel("DoubleMain");

        tiles.Add(new Tile(-1, "", 0.0f, 0.0f));
    }
	
	void Update () {
        //Rect cameraRect = CameraControl.CameraRect;
        double camZoom = CameraControl.zoom;

	    foreach (Tile t in tiles)
        {
            // Update the position of each tile based on the camera zoom/pos
            t.UpdatePosition();

            // Check if we should divide the tile
            if (camZoom * zoomFudge > t.Zoom &&
                t.level < 29 &&     // Magic number: double seems to bottom out around 29
                t.IsInView() &&
                t.splittable)
            {
                t.splittable = false;
                RenderManager.SplitQueue.Add(t);
                //tiles.AddRange(t.buildChildren());
                //t.Destroy();
                //tiles.Remove(t);
                //break;
            }

            else if (CutoffManager.cutoff > t.iterations + 100 &&
                t.splittable) {
                t.RenderDeeper();
            }
       } 

        if (RenderManager.ObsoleteTile != null)
        {
            RenderManager.ObsoleteTile.Destroy();
            tiles.Remove(RenderManager.ObsoleteTile);
        }
        Tile newTile = RenderManager.RenderTile();
        if (newTile != null)
            tiles.Add(newTile);
	}

    public class RenderManager
    {
        public static List<Tile> SplitQueue = new List<Tile>();
        public static Tile ObsoleteTile = null;
        private static int sequence = 0;

        internal static Tile RenderTile()
        {
            switch (sequence)
            {
                case 0:
                    if (SplitQueue.Count == 0)
                        return null;
                    sequence = 1;
                    return SplitQueue[0].buildChild(0, 0);
                case 1:
                    sequence = 2;
                    return SplitQueue[0].buildChild(0, 1);
                case 2:
                    sequence = 3;
                    return SplitQueue[0].buildChild(1, 0);
                case 3:
                    sequence = 0;
                    ObsoleteTile = SplitQueue[0];
                    SplitQueue.RemoveAt(0);
                    return ObsoleteTile.buildChild(1, 1);
                default:
                    throw new Exception(String.Format("Sequence {0} is not legal!", sequence));
            }
        }
    }


    public class Tile
    {
        public int level;
        public string address;
        public double centerX;
        public double centerY;
        private GameObject plane;
        public int iterations;

        public string HexAddress { get
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

        public Tile(int v1, string addr, double v2, double v3)
        {
            level = v1;
            address = addr;
            centerX = v2;
            centerY = v3;

            iterations = (int)CutoffManager.cutoff + 100;

            RenderTexture texture = new RenderTexture(inst.textureSize, inst.textureSize, 32, RenderTextureFormat.RFloat);
            texture.enableRandomWrite = true;
            texture.Create();

            inst.shader.SetInt("textureSize", inst.textureSize);
            inst.shader.SetInt("maxIt", iterations);

            if (level < 17) // Magic number: around zoom 2^17, the float is not enough precision
            {
                inst.shader.SetTexture(inst.floatKernel, "Result", texture);
                inst.shader.SetFloats("bounds", new float[] { (float)Top, (float)Left, (float)Bottom, (float)Right });

                inst.shader.Dispatch(inst.floatKernel, inst.textureSize / 16, inst.textureSize / 16, 1);
            }
            else
            {
                inst.shader.SetTexture(inst.doubleKernel, "Result", texture);

                inst.shader.SetInts("leftInts", DoubleAsInts(Left));
                inst.shader.SetInts("rightInts", DoubleAsInts(Right));
                inst.shader.SetInts("topInts", DoubleAsInts(Top));
                inst.shader.SetInts("bottomInts", DoubleAsInts(Bottom));

                inst.shader.Dispatch(inst.doubleKernel, inst.textureSize / 4, inst.textureSize / 4, 1);
            }

            plane = (GameObject)Instantiate(inst.tilePrefab);
            UpdatePosition();

            plane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);

            //text = plane.GetComponent<GUIText>();
            //text.text = level.ToString();
            TextMesh text = plane.GetComponentInChildren<TextMesh>();
            if (text != null)
            {

                text.text = address;
                //text.fontSize /= level;

            }
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
            plane.transform.localScale = new Vector3(scale, scale, level / 10.0f);

            //text.transform.position = plane.transform.localScale;
            TextMesh text = plane.GetComponentInChildren<TextMesh>();
            if (text != null)
            {


                //    text.fontSize = (int)(96 * scale);
                //text.text = address;
                text.text = HexAddress;
                if (level > 0)
                    text.fontSize = (int)(400 / Math.Pow(HexAddress.Length, 0.7));
            //    //text.transform.position = Vector3.Scale(plane.transform.position, new Vector3(scale, scale, scale));

        }
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

            return new Tile(level + 1, address + addressNext, cx, cy);
        }

        public void RenderDeeper()
        {
            RenderTexture tex = plane.GetComponent<MeshRenderer>().material.GetTexture("_MainTex") as RenderTexture;

            if (tex == null)
                throw new Exception("Could not cast the texture as a RenderTexture!");

            inst.shader.SetInt("textureSize", inst.textureSize);
        }

        internal void Destroy()
        {
            if (plane != null)
            {
                var text = plane.GetComponentInChildren<TextMesh>();
                if (text != null)
                    text.color = new Color(0, 0, 0, 0);
            }
            //GameObject.Destroy()
            //foreach (object obj in plane.transform)
            //{
            //    var t = (obj as Transform);
            //    if (t != null)
            //        GameObject.Destroy(t.gameObject);
            //    //if (child != null)
            //    //    GameObject.Destroy(child);
            //}
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
    }
}
