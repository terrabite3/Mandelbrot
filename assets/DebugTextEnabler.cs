using UnityEngine;
using System.Collections;

public class DebugTextEnabler : MonoBehaviour {

    public bool DebugModeOn = false;

    public GameObject TextPrecon;
    private GameObject TextObject;

    // Use this for initialization
    void Start () {
        if (DebugModeOn)
        {
            TextObject = Instantiate<GameObject>(TextPrecon);

            TextObject.transform.parent = gameObject.transform;


        }
	}
	
	// Update is called once per frame
	void Update () {
        //GetComponent<GUIText>().enabled = DebugModeOn;
        
        if (DebugModeOn)
        {
            if (TextObject != null)
            {
                var mesh = TextObject.GetComponent<TextMesh>();

                //mesh.fontSize = (int)(96 * scale);
                //mesh.text = level.ToString();
                //text.transform.position = Vector3.Scale(plane.transform.position, new Vector3(scale, scale, scale));

                TextObject.transform.position = gameObject.transform.position;
                //TextObject.transform.localScale = gameObject.transform.localScale / 100;
            }
        }
	}
}
