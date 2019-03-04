using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    Animator myAnimator;

    private bool open = false;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update() {
        if (!open) {
            if (Input.GetKeyDown(KeyCode.O)) {
                myAnimator.SetTrigger("Open");
                open = !open;
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.C)) {
                myAnimator.SetTrigger("Close");
                open = !open;
            }
        }
    }
}
