using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour {

    public bool DebugMode = false;

    public static float PixelThreshold
    {
        get
        {
            int height = Screen.height;

            return height / 500;
        }
    }
    
	void Start () {
	
	}
	
	void Update () {
        if (Input.GetButtonDown("Debug"))
            DebugMode = !DebugMode;
	}
}
