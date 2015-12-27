using UnityEngine;
using System.Collections;

public class DebugTextScript : MonoBehaviour {

    private static readonly Color ENABLED_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private static readonly Color DISABLED_COLOR = new Color(0, 0, 0, 0);

    private Tile parent;
    private TextMesh mesh;
    private GlobalScript globals;
    
	void Start () {
        globals = GameObject.Find("Globals").GetComponent<GlobalScript>();

        parent = GetComponentInParent<Tile>();
        mesh = GetComponent<TextMesh>();

        mesh.text = parent.HexAddress;
        GetComponent<TextMesh>().fontSize = 1000 / parent.HexAddress.Length;
    }
	
	void Update () {
        if (globals.DebugMode)
            mesh.color = ENABLED_COLOR;
        else
            mesh.color = DISABLED_COLOR;
        
        gameObject.transform.position = parent.gameObject.transform.position;
    }
}
