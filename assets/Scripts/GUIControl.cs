using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {

    private Canvas canvas;

	// Use this for initialization
	void Start () {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    
}
