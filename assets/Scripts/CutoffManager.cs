using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CutoffManager : MonoBehaviour {

    public static float cutoff = 0.1f;
    public float cutoffVelocity = 0.01f;
    public float cutoffSlowness = 10.0f;

    public UnityEvent updateVelocityEvent;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Faster"))
        {
            cutoffVelocity += 0.005f;
            updateVelocityEvent.Invoke();
            GameObject.Find("VelocitySlider").GetComponent<MonoBehaviour>().SendMessage("Update", cutoffVelocity);
        }
        if (Input.GetButtonDown("Slower"))
        {
            cutoffVelocity -= 0.005f;
            updateVelocityEvent.Invoke();
        }

        float t = Input.GetAxis("Depth");

        cutoff += t / cutoffSlowness;

        cutoff += cutoffVelocity;

        if (cutoff > 1000)
        {
            Debug.Log("Cutoff is beyond 1000.");
            cutoff -= 1;
        }
    }

    public void Reset()
    {
        cutoff = 0;
    }
}
