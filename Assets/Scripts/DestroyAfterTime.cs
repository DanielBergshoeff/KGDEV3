using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float timeTillDestroy = 1.0f;

	// Use this for initialization
	void Start () {
        Invoke("SelfDestruct", timeTillDestroy);
	}

    private void SelfDestruct() {
        Destroy(this.gameObject);
    }
}
