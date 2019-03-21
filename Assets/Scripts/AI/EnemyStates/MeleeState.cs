using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : State {

    private EnemyAI enemyAI;

    private float timeNotSeen = 0.0f;
    private Vector3 positionSeen = Vector3.zero;
    private Vector3 targetDirection = Vector3.zero;

    public MeleeState(EnemyAI enemyAI) {
        this.enemyAI = enemyAI;
    }

    public override void StateStart() {

    }

    public override void StateBehaviour() {
        if (enemyAI.hitTimer > 0.0f)
            enemyAI.hitTimer -= Time.deltaTime;

        //Increase time not seen by the time it took to complete the last frame
        timeNotSeen += Time.deltaTime;

        //Check whether the entity can see the target
        Vector3 heading = EnemyAI.target.position - enemyAI.transform.position;
        float distanceTarget = heading.magnitude;
        Vector3 direction = heading / distanceTarget;

        //Rotate towards the position in which the target was last seen
        Vector3 newDir = Vector3.RotateTowards(enemyAI.transform.forward, positionSeen - enemyAI.transform.position, Time.deltaTime, 0.0f);
        enemyAI.transform.rotation = Quaternion.LookRotation(newDir);

        float angleTarget = Vector3.Angle(heading, enemyAI.transform.forward);
        if (!Physics.Raycast(enemyAI.transform.position, direction, distanceTarget, GameManager.gameManager.WallMask) && distanceTarget <= enemyAI.ViewRange && angleTarget <= enemyAI.ViewAngle) {
            //If it can
            timeNotSeen = 0.0f; //Set the timeNotSeen to 0
            positionSeen = EnemyAI.target.position; //Save the position in which the target has been seen
            targetDirection = EnemyAI.target.forward; //Save the direction the target is facing
        }

        if (enemyAI.hitTimer <= 0.0f) {
            //If the target has been seen
            if (positionSeen != Vector3.zero) {
                //If the target is within hit distance
                if (Vector3.Distance(enemyAI.transform.position, positionSeen) <= enemyAI.hitDistance) {
                    enemyAI.hitTimer = enemyAI.timePerShot;

                    GameObject enemyLightObject = Object.Instantiate(enemyAI.BigLightPrefab, enemyAI.transform);
                    enemyLightObject.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);

                    //HIT PLAYER
                }
                else {
                    enemyAI.CurrentPath = enemyAI.pathfinding.FindPath(enemyAI.transform.position, positionSeen);
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
