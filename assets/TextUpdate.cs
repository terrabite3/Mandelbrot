using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        string result = "";

        result += string.Format("Zoom: {0:0.000E0}\n", CameraControl.zoom);
        result += string.Format("Iterations: {0:f2}\n", CutoffManager.cutoff);

        GetComponent<Text>().text = result;
	}
}
