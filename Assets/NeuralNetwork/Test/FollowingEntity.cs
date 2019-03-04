using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEntity : MonoBehaviour {

    private CharacterController myCharacterController;
    private bool initialized = false;
    private NeuralNetwork neuralNetwork;

    private Transform player;
    private float force = 100.0f;
    
	void Start () {
        myCharacterController = GetComponent<CharacterController>();
	}
	
	void FixedUpdate () {
        if (initialized) {
            float[] outputs = SendInputsToNeuralNetwork();
            /*
            //Accelerate based on output 0
            myRigidBody.AddForce(transform.forward * outputs[0] * force);
            //Decelerate based on output 1
            myRigidBody.AddForce(-transform.forward * outputs[1] * force);
            */
            //Go to value output 0 on the x-axis
            //Go to value output 1 on the z axis
            var direction = new Vector3(outputs[0], 0f, outputs[1]);
            myCharacterController.Move(direction.normalized * Time.deltaTime  * 5f);

            float fitness = 1 - Mathf.Clamp(Vector3.Distance(player.position, transform.position), 0, 100) / 100;
            neuralNetwork.AddFitness(fitness);
        }
    }

    /// <summary>
    /// Sends inputs to the neural network
    /// </summary>
    private float[] SendInputsToNeuralNetwork() {
        //Every entity has 8 inputs for the neural network
        float[] inputs = new float[4];
        //Its own position's x value
        inputs[0] = transform.position.x;
        //Its own position's z value
        inputs[1] = transform.position.z;
        //The player's position's x value
        inputs[2] = player.position.x;
        //The player's position's z value
        inputs[3] = player.position.z;
        /*
        //The player's rotation's y value
        inputs[4] = player.rotation.y;
        //Its own velocity on the x axis
        inputs[5] = myRigidBody.velocity.x;
        //Its own velocity on the z axis
        inputs[6] = myRigidBody.velocity.z;
        //The distance of the nearest object in front of it
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f)){
            inputs[7] = hit.distance;
        }
        else {
            inputs[7] = 100.0f;
        }
        */

        return neuralNetwork.FeedForward(inputs);
    }

    public void Init(NeuralNetwork neuralNetwork, Transform player) {
        this.neuralNetwork = neuralNetwork;
        this.player = player;
        initialized = true;
    }

    public void AddFitness(float value) {
        neuralNetwork.AddFitness(value);
    }
}
