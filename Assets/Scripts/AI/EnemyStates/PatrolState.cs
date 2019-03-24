using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State {

    private EnemyAI EnemyAI;
    private List<Node> GridPortion;

    public PatrolState(EnemyAI EnemyAI, List<Node> GridPortion) {
        this.EnemyAI = EnemyAI;
        this.GridPortion = GridPortion;
    }

    public override void StateStart() {

    }

    public override void StateBehaviour() {
        EnemyAI.timer += Time.deltaTime;

        UpdateNodes();

        if (EnemyAI.timer > EnemyAI.timePerCalculation) {
            EnemyAI.timer = 0.0f;

            float timeNotSeen = 0.0f;
            Node node = null;

            foreach (Node n in GridPortion) {
                if (n.bIsWall == true) { //If the node is not a wall
                    if (n.timeNotSeen >= timeNotSeen) { //If this node has not been seen for a longer time than current node
                        node = n;
                        timeNotSeen = node.timeNotSeen;
                    }
                }
            }
            EnemyAI.SetTargetPosition(node.vPosition);
        }

        EnemyAI.MoveAlongPath(EnemyAI.walkSpeed);
    }

    public override void StateEnd() {

    }

    /// <summary>
    /// If a node is within vision of the entity that is patrolling the area, its timeNotSeen variable is set back to 0
    /// </summary>
    /// <param name="enemyIndex"></param>
    private void UpdateNodes() {
        for (int j = 0; j < GridPortion.Count; j++) {
            Vector3 heading = GridPortion[j].vPosition - EnemyAI.transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;
            float angle = Vector3.Angle(heading, EnemyAI.transform.forward);
            if (!Physics.Raycast(EnemyAI.transform.position, direction, distance, GameManager.gameManager.WallMask) && distance <= EnemyAI.ViewRange && angle <= EnemyAI.ViewAngle) {
                GridPortion[j].timeNotSeen = 0.0f;
            }
        }
    }
}

