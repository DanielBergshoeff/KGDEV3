using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestPlayer : MonoBehaviour {

    private NavMeshAgent navMeshAgent;
    private Vector3 target = Vector3.zero;
    private bool targetFound = true;

	// Use this for initialization
	void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
	}

    // Update is called once per frame
    void Update() {
        if (targetFound) {
            target = GetRandomPosition();
            navMeshAgent.SetDestination(target);
            targetFound = false;
        }
        else {
            if(Vector3.Distance(transform.position, target) < 0.1f) {
                targetFound = true;
            }
        }
    }

    private Vector3 GetRandomPosition() {
        Vector3 tempRange = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0.0f, UnityEngine.Random.Range(-10f, 10f));
        Vector3 tempTarget = transform.position + tempRange;
        NavMeshPath path = new NavMeshPath();
        if (navMeshAgent.CalculatePath(tempTarget, path))
            return tempTarget;
        else
            return GetRandomPosition();
    }
}
