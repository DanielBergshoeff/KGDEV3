using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour {

    private GameObject target;

	// Use this for initialization
	void Start () {
        target = GameObject.Find("FPSController");
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.LookRotation(transform.position - target.transform.position);
	}
}
