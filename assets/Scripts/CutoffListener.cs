using UnityEngine;
using System.Collections;

public class CutoffListener : MonoBehaviour {

    private Material material;

	// Use this for initialization
	void Start () {
        material = GetComponent<MeshRenderer>().material;

        material.GetTexture("_MainTex").filterMode = FilterMode.Point;
    }
	
	// Update is called once per frame
	void Update () {
        material.SetFloat("_Cutoff", CutoffManager.cutoff);
    }
}
