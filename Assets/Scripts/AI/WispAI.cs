using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispAI : MonoBehaviour {

    public StateMachine stateMachine;

    public float Speed = 3.0f;
    public NeuralNetwork neuralNetworkRoam;
    public NeuralNetwork neuralNetworkGathering;

    public Transform targetPosition;

    public List<LightTimer> lightTimers;

    //Roam state
    public float forwardRaycastDistance = 1000.0f;

    // Use this for initialization
    void Start () {
        lightTimers = new List<LightTimer>();
        stateMachine = new StateMachine();
        stateMachine.SwitchState(new GatheringState(this));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        if (Physics.OverlapSphere(transform.position, 0.5f).Length > 0)
            SelfDestroy(colliders);

        if (stateMachine != null)
            stateMachine.ExecuteState();

        for (int i = 0; i < lightTimers.Count; i++) {
            lightTimers[i].timer -= Time.deltaTime;
            if (lightTimers[i].timer <= 0.0f)
                lightTimers.Remove(lightTimers[i]);
        }
    }

    public void Init(NeuralNetwork nn) {
        neuralNetworkRoam = nn;
    }

    public void InitGathering(NeuralNetwork nn, Transform Target) {
        neuralNetworkGathering = nn;
        targetPosition = Target;
    }

    public void SelfDestroy(Collider[] colliders) {
        if(colliders != null) {
            foreach(Collider col in colliders) {
                if (col.CompareTag("Enemy")) {
                    Debug.Log("Caught!");
                    col.transform.parent.GetComponent<EnemyAI>().Energy += 10.0f;
                }
            }
        }

        WispSpawner.AllWisps.Remove(this);

        Destroy(this.gameObject);
    }

    public void SetFitnessAndDestroy(Collider[] colliders) {
        neuralNetworkGathering.SetFitness(-Vector3.Distance(transform.position, targetPosition.position));
        SelfDestroy(colliders);
    }
}

public class LightTimer {
    public GameObject light;
    public float timer;

    public LightTimer(GameObject light, float timer) {
        this.light = light;
        this.timer = timer;
    }
}
