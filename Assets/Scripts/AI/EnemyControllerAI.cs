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
            AI.GridPortion = gridPortions[i];
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < enemyAIs.Count; i++) {
            for (int j = 0; j < gridPortions[i].Count; j++) {
                Vector3 heading = gridPortions[i][j].vPosition - enemyAIs[i].transform.position;
                float distance = heading.magnitude;
                Vector3 direction = heading / distance;
                float angle = Vector3.Angle(heading, enemyAIs[i].transform.forward);
                if (!Physics.Raycast(enemyAIs[i].transform.position, direction, distance, GameManager.gameManager.WallMask) && distance <= enemyAIs[i].ViewRange && angle <= enemyAIs[i].ViewAngle) {
                    gridPortions[i][j].timeNotSeen = 0.0f;
                }
            }
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
