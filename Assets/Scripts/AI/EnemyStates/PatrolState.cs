using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State {

    private EnemyAI enemyAI;

    public PatrolState(EnemyAI enemyAI) {
        this.enemyAI = enemyAI;
    }

    public override void StateStart() {

    }

    public override void StateBehaviour() {
        enemyAI.timer += Time.deltaTime;
        if (enemyAI.timer > enemyAI.timePerCalculation) {
            enemyAI.timer = 0.0f;

            float timeNotSeen = 0.0f;
            Node node = null;

            foreach (Node n in enemyAI.GridPortion) {
                if (n.bIsWall == true) { //If the node is not a wall
                    if (n.timeNotSeen >= timeNotSeen) { //If this node has not been seen for a longer time than current node
                        node = n;
                        timeNotSeen = node.timeNotSeen;
                    }
                }
            }
            enemyAI.CurrentPath = enemyAI.pathfinding.FindPath(enemyAI.transform.position, node.vPosition);
        }

        enemyAI.MoveAlongPath(enemyAI.walkSpeed);
    }

    public override void StateEnd() {

    }
}
