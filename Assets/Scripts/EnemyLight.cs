using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLight : MonoBehaviour {

    public EnemyAI owner;
    public Rigidbody myRigidbody;

    private void Start() {

    }

    private void Update() {
        //If the light leaves the bounds of the room, call selfdestruct method
        if (transform.position.x < RangedNeuralNetwork.Instance.transform.position.x - RangedNeuralNetwork.Instance.roomSize.x / 2 ||
           transform.position.x > RangedNeuralNetwork.Instance.transform.position.x + RangedNeuralNetwork.Instance.roomSize.x / 2 ||
           transform.position.y < RangedNeuralNetwork.Instance.transform.position.y - RangedNeuralNetwork.Instance.roomSize.y / 2 ||
           transform.position.y > RangedNeuralNetwork.Instance.transform.position.y + RangedNeuralNetwork.Instance.roomSize.y / 2 ||
           transform.position.z < RangedNeuralNetwork.Instance.transform.position.z - RangedNeuralNetwork.Instance.roomSize.z / 2 ||
           transform.position.z > RangedNeuralNetwork.Instance.transform.position.z + RangedNeuralNetwork.Instance.roomSize.z / 2
            )
            SelfDestruct();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player hit!");
        }
        
        SelfDestruct();
    }

    public void SelfDestruct() {
        owner.neuralNetwork.SetFitness(1 - Vector3.Distance(transform.position, RangedNeuralNetwork.Player.transform.position) / 50);
        owner.bulletArrived = true;
        Destroy(this.gameObject);
    }
}
