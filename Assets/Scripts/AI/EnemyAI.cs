using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public List<Node> CurrentPath;//The completed path that the red line will be drawn along


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (CurrentPath != null) {
            if (CurrentPath.Count > 0) {
                if (CurrentPath[0] != null) {
                    if (Vector3.Distance(CurrentPath[0].vPosition, transform.position) < 0.01f) {
                        CurrentPath.RemoveAt(0);
                    }
                    else
                        transform.position = Vector3.MoveTowards(transform.position, CurrentPath[0].vPosition, Time.deltaTime);
                }
            }
        }
	}
}
