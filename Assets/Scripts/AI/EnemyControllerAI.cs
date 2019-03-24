using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerAI : MonoBehaviour {

    public Grid Grid;
    public Pathfinding Pathfinding;
    public NeuralNetwork NeuralNetwork;

    [SerializeField] private List<EnemyAI> enemyAIs;
    private List<List<Node>> gridPortions;
    private List<Color> gridColors;

    private bool playerSeen = false;
    private Vector3 positionPlayerSeen;
    private float timeNotSeen = 0.0f;

    

    // Use this for initialization
    void Start () {
        gridPortions = Grid.CreateGridPortions(enemyAIs.Count);

        gridColors = new List<Color>();
        for (int i = 0; i < gridPortions.Count; i++) {
            gridColors.Add(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
        }
        for (int i = 0; i < enemyAIs.Count; i++) {
            EnemyAI AI = enemyAIs[i].GetComponent<EnemyAI>();
            AI.pathfinding = Pathfinding;
            AI.stateMachine = new StateMachine();
            AI.GridPortion = gridPortions[i];
            AI.stateMachine.SwitchState(new PatrolState(AI, gridPortions[i]));
        }
    }
	
	// Update is called once per frame
	void Update () {
        bool checkPlayerSeen = false;
        foreach(EnemyAI enemyAI in enemyAIs) {
            if (enemyAI.PlayerSeen) {
                checkPlayerSeen = true;
                playerSeen = true;
                positionPlayerSeen = enemyAI.PlayerPositionSeen; //Update the PlayerPosition
            }

            if (enemyAI.Energy <= 10.0f) { //If the AI is running too low on energy to fight
                if(enemyAI.stateMachine.currentState.GetType() != typeof(WispGatheringState)){
                    Debug.Log("Emergency energy acquisition state");
                    enemyAI.stateMachine.SwitchState(new WispGatheringState(enemyAI));
                }
            }
            else if (enemyAI.PlayerSeen) { //If the AI can see the player
                if (Vector3.Distance(positionPlayerSeen, enemyAI.transform.position) > 15.0f) {
                    enemyAI.stateMachine.SwitchState(new RangedState(enemyAI, positionPlayerSeen)); //Switch to ranged state
                    Debug.Log("Ranged state");
                }
                else {
                    enemyAI.stateMachine.SwitchState(new MeleeState(enemyAI, positionPlayerSeen)); //Switch to melee state
                    Debug.Log("Melee state");
                }
            }
            else if (playerSeen && timeNotSeen <= 1.0f) { //If the enemy AI cannot currently see the player, but the player has been spotted
                enemyAI.stateMachine.SwitchState(new RepositionState(enemyAI, positionPlayerSeen)); //Reposition to the position in which the player has last been seen
                Debug.Log("Reposition state");
            }
            else if (playerSeen) { //If the player has been spotted and AI is not currently already searching
                if (enemyAI.stateMachine.currentState.GetType() != typeof(SearchState)) {
                    enemyAI.stateMachine.SwitchState(new SearchState(enemyAI, timeNotSeen, positionPlayerSeen));
                    Debug.Log("Search state");
                }
            }
            else if(enemyAI.Energy <= 70.0f) { //If the AI's energy is lower than 70%
                if (enemyAI.stateMachine.currentState.GetType() != typeof(WispGatheringState)) {
                    enemyAI.stateMachine.SwitchState(new WispGatheringState(enemyAI));
                    Debug.Log("Energy acquisition state");
                }
            }
            else if(enemyAI.stateMachine.currentState.GetType() != typeof(PatrolState)){ //If there is nothing more important to do
                enemyAI.stateMachine.SwitchState(new PatrolState(enemyAI, enemyAI.GridPortion));
                Debug.Log("Patrol state");
            }
        }
        if (checkPlayerSeen) {
            timeNotSeen = 0.0f;
        }
        else {
            timeNotSeen += Time.deltaTime;
        }
    }

    public void Init(NeuralNetwork neuralNetwork) {
        this.NeuralNetwork = neuralNetwork;
    }

    //Function that draws the wireframe
    private void OnDrawGizmos() {

        if (gridPortions != null) {
            for (int i = 0; i < gridPortions.Count; i++) {
                if (gridPortions[i] != null) {
                    foreach (Node n in gridPortions[i]) {
                        if (n.bIsWall) {
                            Gizmos.color = new Color(gridColors[i].r, gridColors[i].g, gridColors[i].b, n.timeNotSeen / 25f); //Set the color of the node
                        }
                        else {
                            Gizmos.color = Color.black;
                        }

                        Gizmos.DrawCube(n.vPosition, Vector3.one * (Grid.fNodeDiameter - Grid.fDistanceBetweenNodes));//Draw the node at the position of the node.
                    }
                }
            }
        }
    }
}
