using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : State {

    private EnemyAI enemyAI;
    private Transform target;

    private float timeNotSeen = 0.0f;
    private float timer = 0.0f;
    private Vector3 positionSeen = Vector3.zero;
    private bool enemySeen = false;

    private Node startNode;
    private List<Node> OpenList;
    private List<Node> ClosedList;

    public SearchState(EnemyAI enemyAI) {
        this.enemyAI = enemyAI;
    }

    public override void StateStart() {
        OpenList = new List<Node>();
        ClosedList = new List<Node>();

    }

    public override void StateBehaviour() {
        //Increase time not seen by the time it took to complete the last frame
        timeNotSeen += Time.deltaTime;

        //Check whether the entity can see the target
        Vector3 heading = target.position - enemyAI.transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        float angle = Vector3.Angle(heading, enemyAI.transform.forward);
        //If it can
        if (!Physics.Raycast(enemyAI.transform.position, direction, distance) && distance <= enemyAI.ViewRange && angle <= enemyAI.ViewAngle) {
            timeNotSeen = 0.0f; //Set the timeNotSeen to 0
            positionSeen = target.position; //Save the position in which the target has been seen
            enemySeen = true;

            OpenList = new List<Node>();
            ClosedList = new List<Node>();

            startNode = enemyAI.pathfinding.GridReference.NodeFromWorldPoint(positionSeen);
            OpenList.Add(startNode);
            AddNeighbourNodesToList();
        }

        List<Node> NodesToRemove = new List<Node>();
        for (int i = 0; i < OpenList.Count; i++) {
            Vector3 headingNode = target.position - enemyAI.transform.position;
            float distanceNode = heading.magnitude;
            Vector3 directionNode = heading / distance;
            float angleNode = Vector3.Angle(heading, enemyAI.transform.forward);
            if (!Physics.Raycast(enemyAI.transform.position, directionNode, distanceNode) && distanceNode <= enemyAI.ViewRange && angleNode <= enemyAI.ViewAngle) {
                NodesToRemove.Add(OpenList[i]);
            }
        }

        for (int i = 0; i < NodesToRemove.Count; i++) {
            ClosedList.Add(NodesToRemove[i]);
            OpenList.Remove(NodesToRemove[i]);
        }

        //If the target has been seen
        if (enemySeen) {
            if (timeNotSeen > 0.0f) {
                if (OpenList != null) {
                    if (OpenList.Count > 0) {
                        enemyAI.CurrentPath = enemyAI.pathfinding.FindPath(enemyAI.transform.position, OpenList[0].vPosition);
                    }

                    enemyAI.MoveAlongPath(enemyAI.walkSpeed);
                }
                
                timer += Time.deltaTime;
                if (timer > 1.0f) {
                    timer = 0.0f;
                    AddNeighbourNodesToList();
                }
            }
        }
    }

    private void AddNeighbourNodesToList() {
        for (int i = 0; i < OpenList.Count; i++) {
            List<Node> nodes = enemyAI.pathfinding.GridReference.GetNeighboringNodes(OpenList[i]);
            for (int j = 0; j < nodes.Count; j++) {
                if (nodes[j].bIsWall && !OpenList.Contains(nodes[j]) && !ClosedList.Contains(nodes[j]))
                    OpenList.Add(nodes[j]);
            }
        }
    }

    public override void StateEnd() {

    }
}
