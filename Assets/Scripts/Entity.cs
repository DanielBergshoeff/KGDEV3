using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityValues {
    public float distanceForJump = 3.0f;
    public float speed = 1000.0f;
}

public class Entity : MonoBehaviour {

    public EntityValues entityValues;

    public float maxSpeed = 300.0f;
    public float drag = 0.01f;
    public LayerMask layerObstruction;

    private Rigidbody myRigidBody;
    private bool isGrounded;

    public static GameObject goal;
    private bool goalReached;
    public float timer;

	// Use this for initialization
	void Start () {
        entityValues = new EntityValues();
        myRigidBody = GetComponent<Rigidbody>();
        goal = GameObject.Find("Goal");
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!goalReached) {
            timer += Time.deltaTime;

            if (Physics.Raycast(transform.position, -transform.up, 0.51f)) {
                isGrounded = true;
            }
            else {
                isGrounded = false;
            }

            myRigidBody.AddForce(transform.forward * entityValues.speed * Time.deltaTime);

            if (isGrounded) {
                if (Physics.Raycast(transform.position, transform.forward, transform.localScale.x / 2 + entityValues.distanceForJump, layerObstruction)) {
                    myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, 0.0f, myRigidBody.velocity.z);
                    myRigidBody.AddForce(Vector3.up * 400f);
                }
            }
            if (!isGrounded) {
                myRigidBody.velocity = new Vector3(myRigidBody.velocity.x * (1.0f - drag), myRigidBody.velocity.y, myRigidBody.velocity.z * (1.0f - drag));
            }

            myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, maxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject == goal) {
            myRigidBody.velocity = Vector3.zero;
            goalReached = true;
        }
    }


}
