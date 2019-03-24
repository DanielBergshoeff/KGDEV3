using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : State {

    private EnemyAI enemyAI;
    
    private Vector3 positionSeen = Vector3.zero;

    public MeleeState(EnemyAI enemyAI, Vector3 positionSeen) {
        this.enemyAI = enemyAI;
        this.positionSeen = positionSeen;
    }

    public override void StateStart() {

    }

    public override void StateBehaviour() {
        if (enemyAI.hitTimer > 0.0f)
            enemyAI.hitTimer -= Time.deltaTime;

        if (enemyAI.hitTimer <= 0.0f) {
            //If the target has been seen
            if (positionSeen != Vector3.zero) {
                //If the target is within hit distance
                if (Vector3.Distance(enemyAI.transform.position, positionSeen) <= enemyAI.hitDistance) {
                    enemyAI.hitTimer = enemyAI.timePerShot;

                    GameObject enemyLightObject = Object.Instantiate(enemyAI.BigLightPrefab, enemyAI.transform);
                    enemyLightObject.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
                }
                else {
                    enemyAI.targetPosition = positionSeen;
                    //If not and the target is within sprint distance -> start sprinting
                    if (Vector3.Distance(enemyAI.transform.position, positionSeen) <= enemyAI.sprintDistance)
                        enemyAI.MoveAlongPath(enemyAI.sprintSpeed);
                    //If not and the target is not within sprint distance -> walk towards target
                    else
                        enemyAI.MoveAlongPath(enemyAI.walkSpeed);
                }
            }
        }
    }

    public override void StateEnd() {

    }
}
