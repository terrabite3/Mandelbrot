using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour {

    public bool DebugMode = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Debug"))
            DebugMode = !DebugMode;
	}
}
