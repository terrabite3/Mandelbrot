using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileManager : MonoBehaviour {

    public ComputeShader shader;
    public GameObject tilePrefab;
    public int floatKernel, doubleKernel;
    public int textureSize = 512;

    private List<Tile> tiles = new List<Tile>();
    
	void Start () {

        floatKernel = shader.FindKernel("FloatMain");
        doubleKernel = shader.FindKernel("DoubleMain");
        
        var tile = Instantiate<GameObject>(tilePrefab);
        tiles.Add(tile.GetComponent<Tile>());
        var tileScript = tile.GetComponent<Tile>();
        tileScript.Init(this, -1, "", 0.0f, 0.0f);
    }
	
	void Update () {
        double camZoom = CameraControl.zoom;

	    foreach (Tile t in tiles)
        {

            // Check if we should divide the tile
            if (camZoom * GlobalScript.PixelThreshold > t.Zoom &&
                t.level < 29 &&     // Magic number: double seems to bottom out around 29
                t.IsInView() &&
                t.splittable)
            {
                t.splittable = false;
                RenderManager.SplitQueue.Add(t);
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

}
