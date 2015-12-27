using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class CameraControl : MonoBehaviour {

	public double initZoom = 1.1;
    public static double zoom = 1.1;
    public static double centerX;
    public static double centerY;
	private double initSize;
    private static Camera cam;
    public float panSlowness = 100.0f;
    public float zoomSlowness = 100.0f;

    public static Rect CameraRect { get
        {
            float aspectRatio = cam.aspect;
            //return new Rect(
            //    (float)centerX, 
            //    (float)centerY, 
            //    aspectRatio * 2 / (float)zoom, 
            //    2 / (float)zoom);
            return new Rect(
                (float)(centerX - aspectRatio / zoom),
                (float)(centerY - 1 / zoom),
                (float)(aspectRatio * 2 / zoom),
                (float)(2 / zoom)
                );
        }
    }
    
	void Start () {
		cam = GetComponent<Camera> ();
        initSize = cam.orthographicSize;
	}
	
	void Update () {
		float x = Input.GetAxis ("PanX");
		float y = Input.GetAxis ("PanY");
		float h = Input.GetAxis ("Zoom");
        
        centerX += x / panSlowness / zoom;
        centerY += y / panSlowness / zoom;
        //cam.transform.position = new Vector3((float)centerX / 2, (float)centerY / 2, cam.transform.position.z);

        zoom *= 1 + h / zoomSlowness;
        //cam.orthographicSize = (float)(initSize / zoom);
        // By scaling the tiles and zooming the camera each at sqrt(zoom),
        // we square the maximum zoom using floats
        //cam.orthographicSize = (float)(initSize / Math.Sqrt(zoom));
    }

    public void Reset()
    {
        zoom = initZoom;
        centerX = 0;
        centerY = 0;
    }
}
