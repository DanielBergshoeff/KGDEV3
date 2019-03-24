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

    public SearchState(EnemyAI enemyAI, float timeNotSeen, Vector3 positionSeen) {
        this.enemyAI = enemyAI;
        this.timeNotSeen = timeNotSeen;
        this.positionSeen = positionSeen;
    }

    public override void StateStart() {
        OpenList = new List<Node>();
        ClosedList = new List<Node>();

        startNode = enemyAI.pathfinding.GridReference.NodeFromWorldPoint(positionSeen);
        OpenList.Add(startNode);
        for (int i = 0; i < (int)timeNotSeen; i++) {
            AddNeighbourNodesToList();
        }
    }

    public override void StateBehaviour() {
        List<Node> NodesToRemove = new List<Node>();
        for (int i = 0; i < OpenList.Count; i++) {
            Vector3 headingNode = OpenList[i].vPosition - enemyAI.transform.position;
            float distanceNode = headingNode.magnitude;
            Vector3 directionNode = headingNode / distanceNode;
            float angleNode = Vector3.Angle(directionNode, enemyAI.transform.forward);
            if (!Physics.Raycast(enemyAI.transform.position, directionNode, distanceNode) && distanceNode <= enemyAI.ViewRange && angleNode <= enemyAI.ViewAngle) {
                NodesToRemove.Add(OpenList[i]);
            }
        }

        for (int i = 0; i < NodesToRemove.Count; i++) {
            ClosedList.Add(NodesToRemove[i]);
            OpenList.Remove(NodesToRemove[i]);
        }

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

    private void AddNeighbourNodesToList() {
        int nodesCount = OpenList.Count;
        for (int i = 0; i < nodesCount; i++) {
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
