using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public List<Node> GridPortion;
    public List<Node> CurrentPath;//The completed path that the red line will be drawn along
    public float walkSpeed = 3.0f;
    public float Health {
        get { return health; }
        set {
            health = value;
        }
    }

    private float health = 100.0f;
    
    private Animator animator;

    //Guard
    public float distanceToTurn = 10.0f;

    private Vector3 directionVectorLeft;
    private Vector3 directionVectorForward;

    private StateMachine stateMachine;

    public float ViewRange = 10.0f;
    public float ViewAngle = 45.0f;

    public NeuralNetwork neuralNetwork;

    //Ranged state
    public GameObject EnemyLightPrefab;
    public GameObject BigLightPrefab;
    public static Transform target;
    public Transform myTarget;
    public float timePerShot = 1.0f;
    public float shotTimer = 0.0f;
    public bool bulletArrived = false;

    //Melee state
    public float timePerHit = 1.0f;
    public float hitTimer = 0.0f;
    public float hitDistance = 2.0f;
    public float sprintDistance = 10.0f;
    public float sprintSpeed = 9.0f;
    public Pathfinding pathfinding;

    //Patrolling state
    public float timePerCalculation = 1.0f;
    public float timer = 0.0f;

    public List<GameObject> lights;
    
    // Use this for initialization
    void Start() {
        if (target == null)
            target = myTarget;

        animator = GetComponent<Animator>();
        CurrentPath = new List<Node>();
        stateMachine = new StateMachine();
        stateMachine.SwitchState(new PatrolState(this));
    }

    // Update is called once per frame
    void Update() {
        if(stateMachine != null)
            stateMachine.ExecuteState();
    }

    public void MoveAlongPath(float speed) {
        if (CurrentPath != null) {
            if (CurrentPath.Count > 0) {
                if (CurrentPath[0] != null) {
                    if (Vector3.Distance(CurrentPath[0].vPosition, transform.position) < 0.01f) {
                        transform.LookAt(CurrentPath[0].vPosition);
                        CurrentPath.RemoveAt(0);
                    }
                    else {
                        transform.position = Vector3.MoveTowards(transform.position, CurrentPath[0].vPosition, Time.deltaTime * speed);
                        transform.LookAt(CurrentPath[0].vPosition);
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage) {
        Health -= damage;
    }

    public void InitNeuralNetwork(NeuralNetwork neuralNetwork, Transform target) {
        this.neuralNetwork = neuralNetwork;
        stateMachine = new StateMachine();
        stateMachine.SwitchState(new RangedState(this));
        target = target;
        this.lights = new List<GameObject>();
    }
}
