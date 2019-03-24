using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionState : State {

    private EnemyAI EnemyAI;
    private Vector3 position;

    public RepositionState(EnemyAI EnemyAI, Vector3 position) {
        this.EnemyAI = EnemyAI;
        this.position = position;
    }

    public override void StateBehaviour() {
        EnemyAI.SetTargetPosition(position);
        EnemyAI.MoveAlongPath(EnemyAI.walkSpeed);
    }

    public override void StateEnd() {

    }

    public override void StateStart() {

    }
}
