using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public List<Node> GridPortion;
    public Pathfinding pathfinding;
    public List<Node> CurrentPath;//The completed path that the red line will be drawn along
    public Vector3 targetPosition = Vector3.zero;
    private bool pathChosen = false;
    public float walkSpeed = 3.0f;
    public float Health {
        get { return health; }
        set {
            health = value;
        }
    }

    public bool PlayerSeen = false;
    public Vector3 PlayerPositionSeen;

    public float Energy = 100.0f;

    private float health = 100.0f;
    
    private Animator animator;

    //Guard
    public float distanceToTurn = 10.0f;

    private Vector3 directionVectorLeft;
    private Vector3 directionVectorForward;

    public StateMachine stateMachine;

    public float ViewRange = 20.0f;
    public float ViewAngle = 90.0f;

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

    //Patrolling state
    public float timePerCalculation = 1.0f;
    public float timer = 0.0f;

    public List<GameObject> lights;

    //Wisp gathering
    public static List<GameObject> lightsInUse;
    
    // Use this for initialization
    void Start() {
        if (target == null)
            target = myTarget;
        if (lightsInUse == null)
            lightsInUse = new List<GameObject>();

        animator = GetComponent<Animator>();
        CurrentPath = new List<Node>();
    }

    // Update is called once per frame
    void Update() {
        if(stateMachine != null)
            stateMachine.ExecuteState();

        PlayerCheck();

        //Energy -= Time.deltaTime;
    }

    /// <summary>
    /// If a target position is defined, this method will calculate a path towards it and move the player along it
    /// </summary>
    /// <param name="speed"></param>
    public void MoveAlongPath(float speed) {
        if (CurrentPath != null) {
            if (CurrentPath.Count > 0) {
                if (CurrentPath[0] != null) {
                    if (Vector3.Distance(new Vector3(CurrentPath[0].vPosition.x, transform.position.y, CurrentPath[0].vPosition.z), transform.position) < 0.01f) {
                        transform.LookAt(new Vector3(CurrentPath[0].vPosition.x, transform.position.y, CurrentPath[0].vPosition.z));
                        CurrentPath.RemoveAt(0);
                    }
                    else {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(CurrentPath[0].vPosition.x, transform.position.y, CurrentPath[0].vPosition.z), Time.deltaTime * speed);
                        transform.LookAt(new Vector3(CurrentPath[0].vPosition.x, transform.position.y, CurrentPath[0].vPosition.z));
                    }
                }
            }
            else if (pathChosen) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), Time.deltaTime * speed);
                transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

                if (Vector3.Distance(new Vector3(targetPosition.x, transform.position.y, targetPosition.z), transform.position) < 0.01f) {
                    targetPosition = Vector3.zero;
                    pathChosen = false;
                }
            }

        }
    }

    /// <summary>
    /// If the player is within vision of any of the entities, switch to player focused states
    /// </summary>
    /// <param name="enemyIndex"></param>
    private void PlayerCheck() {
        Vector3 heading = GameManager.gameManager.player.transform.position - transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        float angle = Vector3.Angle(heading, transform.forward);
        if (!Physics.Raycast(transform.position, direction, distance, GameManager.gameManager.WallMask) && distance <= ViewRange && angle <= ViewAngle) {
            PlayerSeen = true;
            PlayerPositionSeen = GameManager.gameManager.player.transform.position;
        }
        else {
            PlayerSeen = false;
        }
    }

    public void SetTargetPosition(Vector3 pos) {
        targetPosition = pos;
        CurrentPath = pathfinding.FindPath(transform.position, targetPosition);
        pathChosen = true;
    }

    public void TakeDamage(float damage) {
        Health -= damage;
        Debug.Log("Damage received");
        if(Health <= 0f) {
            Die();
        }
    }

    public void Die() {
        Destroy(gameObject);
    }

    public void InitNeuralNetwork(NeuralNetwork neuralNetwork, Transform target) {
        this.neuralNetwork = neuralNetwork;
        stateMachine = new StateMachine();
        //stateMachine.SwitchState(new RangedState(this));
        this.lights = new List<GameObject>();
    }
}
