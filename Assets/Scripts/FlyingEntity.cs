using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FlyingEntityValues {
    public float DistanceForCollisionCheck = 10.0f;
    public float DistanceForCollisionLeftAndRight = 30.0f;
    public float Speed = 1800.0f;
}

public class FlyingEntity : MonoBehaviour {
    public FlyingEntityValues entityValues;
    public static GameObject target;
    public GameObject myTarget;

    private Vector3 directionTarget = Vector3.zero;

    public float maxSpeed = 3.0f;
    public float timeForReadjust = 0.3f;
    public float timerForReadjust = 0.0f;
    public Vector3 extraSpace;
    public float drag = 0.01f;
    public LayerMask layerObstruction;

    private Rigidbody myRigidBody;
    private bool isGrounded;
    
    private bool goalReached;
    public bool crashed;
    public float timer;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        timer = 0.0f;
        extraSpace = new Vector3(transform.localScale.x / 2 + transform.localScale.x * 0.5f, 0, 0);

        if (target == null && myTarget != null) {
            target = myTarget;
        }

        directionTarget = target.transform.position - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!goalReached && !crashed) {
            timer += Time.deltaTime;

            if (Physics.Raycast(transform.position, -transform.up, 0.11f)) {
                isGrounded = true;
            }
            else {
                isGrounded = false;
            }

            Debug.DrawRay(transform.position, transform.forward * (transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck));
            Debug.DrawRay(transform.position + extraSpace, transform.forward * (transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck));
            Debug.DrawRay(transform.position - extraSpace, transform.forward * (transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck));

            //If an obstruction is spotted within the DistanceForCollisionCheck range
            if (Physics.Raycast(transform.position, transform.forward, transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck, layerObstruction)
                || Physics.Raycast(transform.position + extraSpace, transform.forward, transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck, layerObstruction)
                || Physics.Raycast(transform.position - extraSpace, transform.forward, transform.localScale.z / 2 + entityValues.DistanceForCollisionCheck, layerObstruction)){

                for (int i = 1; i < 8; i += 2) {
                    timerForReadjust = 0.0f;
                    Vector3 targetVector = Vector3.zero;

                    //Check up
                    var angleVectorUp = Quaternion.AngleAxis(i * -10, transform.right) * transform.forward;
                    bool hitUp = false;
                    RaycastHit hitInfoUp;
                    Debug.DrawRay(transform.position, angleVectorUp * entityValues.DistanceForCollisionLeftAndRight);
                    if (Physics.Raycast(transform.position, angleVectorUp, out hitInfoUp, entityValues.DistanceForCollisionLeftAndRight, layerObstruction)) {
                        hitUp = true;
                    }
                    else {
                        targetVector = angleVectorUp;
                    }

                    //Check to the left
                    var angleVectorLeft = Quaternion.AngleAxis(i * -10, transform.up) * transform.forward;
                    bool hitLeft = false;
                    RaycastHit hitInfoLeft;
                    Debug.DrawRay(transform.position, angleVectorLeft * entityValues.DistanceForCollisionLeftAndRight);
                    if (Physics.Raycast(transform.position, angleVectorLeft, out hitInfoLeft, entityValues.DistanceForCollisionLeftAndRight, layerObstruction)) {
                        hitLeft = true;
                    }
                    else {
                        targetVector = angleVectorLeft;
                    }

                    
                    //Check to the right
                    var angleVectorRight = Quaternion.AngleAxis(i * 10, transform.up) * transform.forward;
                    bool hitRight = false;
                    RaycastHit hitInfoRight;
                    Debug.DrawRay(transform.position, angleVectorRight * entityValues.DistanceForCollisionLeftAndRight);
                    if (Physics.Raycast(transform.position, angleVectorRight, out hitInfoRight, entityValues.DistanceForCollisionLeftAndRight, layerObstruction)) {
                        hitRight = true;
                    }
                    else {
                        targetVector = angleVectorRight;
                    }



                    //If two directions got a hit, but the third hit air OR if only one direction got a hit
                    if ((hitLeft ? 1 : 0) + (hitRight ? 1 : 0) + (hitUp ? 1 : 0) == 1 || (hitLeft ? 1 : 0) + (hitRight ? 1 : 0) + (hitUp ? 1 : 0) == 2){
                        directionTarget = targetVector;
                        break;
                    }
                    else if((hitLeft ? 1 : 0) + (hitRight ? 1 : 0) + (hitUp ? 1 : 0) == 3 && i == 7) {
                        float heighestDistance = Mathf.Max(hitInfoUp.distance, hitInfoLeft.distance, hitInfoRight.distance);

                        if(hitInfoUp.distance == heighestDistance) {
                            targetVector = angleVectorUp;
                        }
                        else if(hitInfoLeft.distance == heighestDistance) {
                            targetVector = angleVectorLeft;
                        }
                        else if(hitInfoRight.distance == heighestDistance) {
                            targetVector = angleVectorRight;
                        }
                    }
                }
            }
            //If no obstruction is spotted
            else {
                timerForReadjust += Time.deltaTime;

                if (timerForReadjust >= timeForReadjust) {
                    Vector3 targetDir = target.transform.position - transform.position;
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 10.0f * Time.deltaTime, 0.0f);
                    if (!Physics.Raycast(transform.position, newDir, 0.5f, layerObstruction) && !Physics.Raycast(transform.position + extraSpace, newDir, 0.5f, layerObstruction) && !Physics.Raycast(transform.position - extraSpace, newDir, 0.5f, layerObstruction))
                        directionTarget = newDir;

                }
            }

            //Veer the current direction so, together with the current velocity, the target direction is achieved
            Vector3 veerVector = (directionTarget - (myRigidBody.velocity.normalized - directionTarget)).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(veerVector, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

            myRigidBody.AddForce(veerVector * entityValues.Speed * Time.deltaTime);
            
            myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, maxSpeed);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject == target) {
            myRigidBody.velocity = Vector3.zero;
            goalReached = true;
        }
        else if(collider.tag == "Obstruction") {
            myRigidBody.velocity = Vector3.zero;
            crashed = true;
        }
    }
}
