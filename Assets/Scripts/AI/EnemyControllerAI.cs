using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerAI : MonoBehaviour {

    public Grid Grid;
    public Pathfinding Pathfinding;
    public float enemyViewRange = 10.0f;
    public float enemyViewAngle = 90.0f;
    public NeuralNetwork NeuralNetwork;

    [SerializeField] private List<EnemyAI> enemyAIs;
    private List<List<Node>> gridPortions;
    private List<Color> gridColors;

    private float currentSimulationSpeed = 1.0f;

    private float timePerCalculation = 1.0f;
    private float timer = 0.0f;

    // Use this for initialization
    void Start () {
        Grid = GetComponent<Grid>();
        Pathfinding = GetComponent<Pathfinding>();

        gridPortions = Grid.CreateGridPortions(enemyAIs.Count);

        gridColors = new List<Color>();
        for (int i = 0; i < gridPortions.Count; i++) {
            gridColors.Add(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
        }
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer > timePerCalculation) {
            timer = 0.0f;

            for (int i = 0; i < enemyAIs.Count; i++) {
                float timeNotSeen = 0.0f;
                Node node = null;

                foreach (Node n in gridPortions[i]) {
                    if (n.bIsWall == true) { //If the node is not a wall
                        if (n.timeNotSeen >= timeNotSeen) { //If this node has not been seen for a longer time than current node
                            node = n;
                            timeNotSeen = node.timeNotSeen;
                        }
                    }
                }

                enemyAIs[i].CurrentPath = Pathfinding.FindPath(enemyAIs[i].transform.position, node.vPosition);
            }


            for (int i = 0; i < enemyAIs.Count; i++) {
                foreach (Node n in Grid.NodeArray) {
                    if (n.bIsWall == true) {


                        Vector3 heading = n.vPosition - enemyAIs[i].transform.position;
                        float distance = heading.magnitude;
                        Vector3 direction = heading / distance;
                        float angle = Vector3.Angle(heading, enemyAIs[i].transform.forward);
                        if (!Physics.Raycast(enemyAIs[i].transform.position, direction, distance, GameManager.gameManager.WallMask) && distance <= enemyViewRange && angle <= enemyViewAngle) {
                            n.timeNotSeen = 0.0f;
                        }
                    }
                }
            }
        }
	}

    public void Init(NeuralNetwork neuralNetwork) {
        this.NeuralNetwork = neuralNetwork;
    }

    //Function that draws the wireframe
    private void OnDrawGizmos() {

        Gizmos.DrawWireCube(transform.position, new Vector3(Grid.vGridWorldSize.x, 1, Grid.vGridWorldSize.y));//Draw a wire cube with the given dimensions from the Unity inspector

        if (gridPortions != null) {
            for (int i = 0; i < gridPortions.Count; i++) {
                if (gridPortions[i] != null) {
                    foreach (Node n in gridPortions[i]) {
                        if (n.bIsWall) {
                            Gizmos.color = gridColors[i]; //Set the color of the node
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
