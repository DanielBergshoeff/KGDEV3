using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallGravityLight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Invoke("Reset", 1.0f);
        }
    }

    private void Reset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
