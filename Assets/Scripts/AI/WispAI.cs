using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispAI : MonoBehaviour {

    private StateMachine stateMachine;

    public float Speed = 3.0f;
    public NeuralNetwork neuralNetworkRoam;
    public NeuralNetwork neuralNetworkGathering;

    public Transform targetPosition;

    //Roam state
    public float forwardRaycastDistance = 1000.0f;

    // Use this for initialization
    void Start () {
        stateMachine = new StateMachine();
        stateMachine.SwitchState(new RoamState(this));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (stateMachine != null)
            stateMachine.ExecuteState();
    }

    public void Init(NeuralNetwork nn) {
        neuralNetworkRoam = nn;
    }

    public void InitGathering(NeuralNetwork nn, Transform Target) {
        neuralNetworkGathering = nn;
        targetPosition = Target;
    }

    public void SelfDestroy() {
        Destroy(this.gameObject);
    }

    public void SetFitnessAndDestroy() {
        neuralNetworkGathering.SetFitness(-Vector3.Distance(transform.position, targetPosition.position));
        SelfDestroy();
    }
}
