using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;

    public LayerMask WallMask;
    public LayerMask ObstructionMask;

	// Use this for initialization
	void Start () {
        if (gameManager == null) {
            gameManager = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
