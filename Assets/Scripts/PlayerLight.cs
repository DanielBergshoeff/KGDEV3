using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour {

    public float startIntensity = 0.1f;
    public float endIntensity = 1.0f;
    public float timeTillFullIntensity = 3.0f;

    public Light lightSource;
    private float timer = 0.0f;

	// Use this for initialization
	void Start () {
        lightSource = GetComponentInChildren<Light>();
        lightSource.intensity = startIntensity;
	}
	
	// Update is called once per frame
	void Update () {
        if (timer < timeTillFullIntensity) {
            timer += Time.deltaTime;
            lightSource.intensity = Mathf.Lerp(startIntensity, endIntensity, timer / timeTillFullIntensity);
        }
	}
}
