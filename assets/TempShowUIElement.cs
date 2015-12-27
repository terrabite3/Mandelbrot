using UnityEngine;
using System.Collections;

public class TempShowUIElement : MonoBehaviour {

    private Canvas canvas;
    public int frameDelay = 60;
    private int frameCountdown = 0;

	// Use this for initialization
	void Start () {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if (frameDelay > 0)
        {
            frameDelay--;
            if (frameDelay == 0)
                Hide();
        }
	}

    public void Show()
    {
        canvas.enabled = true;
        frameCountdown = frameDelay;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Update(Object o)
    {
        //float? value = o as float?;
        //if (value != null)
        //{

        //}
    }
}
