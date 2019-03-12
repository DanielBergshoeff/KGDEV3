using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public enum State {
        Patrol,
        Attack,
        Guard
    }

    public List<Node> CurrentPath;//The completed path that the red line will be drawn along
    public float walkSpeed = 1.0f;
    public Vector3 target;

    //Temporary variables
    public Pathfinding Pathfinding; 
    public Grid Grid; 

    private Animator animator;
    private State currentState;

    //Guard
    public float distanceToTurn = 10.0f;

    private Vector3 directionVectorLeft;
    private Vector3 directionVectorForward;


    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        SwitchState(State.Patrol);
        CurrentPath = new List<Node>();
    }

    // Update is called once per frame
    void Update() {
        switch (currentState) {
            case State.Attack:
                Attack();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Guard:
                Guard();
                break;
        }
    }

    private void SwitchState(State newState) {
        if(newState == State.Guard) {
            StartGuard();
        }

        currentState = newState;
    }

    private void Patrol() {
        if (CurrentPath != null) {
            if (CurrentPath.Count > 0) {
                if (CurrentPath[0] != null) {
                    if (Vector3.Distance(CurrentPath[0].vPosition, transform.position) < 0.01f) {
                        CurrentPath.RemoveAt(0);
                    }
                    else {
                        transform.position = Vector3.MoveTowards(transform.position, CurrentPath[0].vPosition, Time.deltaTime * walkSpeed);
                        transform.LookAt(CurrentPath[0].vPosition);
                    }
                }
            } /*
            else {
                float timeNotSeen = 0.0f;
                Node node = null;

                for (int i = 0; i < Grid.NodeArray.GetLength(0); i++) {
                    for (int j = 0; j < Grid.NodeArray.GetLength(1); j++) {
                        if(Grid.NodeArray[i, j].bIsWall == true) { //If the node is not a wall
                            if(Grid.NodeArray[i, j].timeNotSeen >= timeNotSeen) { //If this node has not been seen for a longer time than current node
                                node = Grid.NodeArray[i, j];
                                timeNotSeen = node.timeNotSeen;
                            }
                        }
                    }
                }

                CurrentPath = Pathfinding.FindPath(transform.position, node.vPosition);
            }*/
        } 
    }

    private void StartGuard() {
        RaycastHit hitForward;
        RaycastHit hitRight;
        RaycastHit hitBack;
        RaycastHit hitLeft;
        RaycastHit closestHit;
        Physics.Raycast(transform.position, transform.forward, out hitForward, 1000.0f, GameManager.gameManager.WallMask);
        Physics.Raycast(transform.position, transform.right, out hitRight, 1000.0f, GameManager.gameManager.WallMask);
        Physics.Raycast(transform.position, -transform.right, out hitLeft, 1000.0f, GameManager.gameManager.WallMask);
        Physics.Raycast(transform.position, -transform.forward, out hitBack, 1000.0f, GameManager.gameManager.WallMask);

        closestHit = (hitForward.distance + hitBack.distance < hitRight.distance + hitLeft.distance) ? hitForward : hitRight;
        directionVectorLeft = new Vector3(closestHit.normal.x, 0, closestHit.normal.z);
        directionVectorForward = new Vector3(closestHit.normal.z, 0, closestHit.normal.x);

        transform.rotation = Quaternion.LookRotation(directionVectorForward, transform.up);
    }

    private void Guard() {
        if (Physics.Raycast(transform.position, transform.forward, distanceToTurn, GameManager.gameManager.WallMask)) {
            directionVectorForward *= -1;
            transform.rotation = Quaternion.LookRotation(directionVectorForward, transform.up);
        }

        transform.position = transform.position + directionVectorForward * Time.deltaTime * walkSpeed;
    }

    private void Attack() {
        transform.LookAt(target);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Aim", true);
    }
}
