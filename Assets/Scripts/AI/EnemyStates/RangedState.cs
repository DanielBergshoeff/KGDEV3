using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : State {

    private EnemyAI enemyAI;

    private float timeNotSeen = 0.0f;
    private Vector3 positionSeen;

    public RangedState(EnemyAI enemyAI, Vector3 positionSeen) {
        this.enemyAI = enemyAI;
        this.positionSeen = positionSeen;
    }

    public override void StateStart() {

    }

    public override void StateBehaviour() {
        if(enemyAI.shotTimer > 0.0f)
            enemyAI.shotTimer -= Time.deltaTime;

        //Rotate towards the position in which the target was last seen
        Vector3 newDir = Vector3.RotateTowards(enemyAI.transform.forward, positionSeen - enemyAI.transform.position, Time.deltaTime, 0.0f);
        enemyAI.transform.rotation = Quaternion.LookRotation(newDir);

        if(enemyAI.shotTimer <= 0.0f) {
            //If the target has been seen
            if (positionSeen != Vector3.negativeInfinity) {
                enemyAI.shotTimer = enemyAI.timePerShot;

                GameObject enemyLightObject = Object.Instantiate(enemyAI.EnemyLightPrefab, enemyAI.transform);
                enemyLightObject.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);

                Vector3 p = positionSeen;

                float gravity = Physics.gravity.magnitude;
                // Selected angle in radians
                float angle = 15.0f * Mathf.Deg2Rad;

                // Positions of this object and the target on the same plane
                Vector3 planarTarget = new Vector3(p.x, 0, p.z);
                Vector3 planarPosition = new Vector3(enemyLightObject.transform.position.x, 0, enemyLightObject.transform.position.z);

                // Planar distance between objects
                float distance = Vector3.Distance(planarTarget, planarPosition);
                // Distance along the y axis between objects
                float yOffset = enemyLightObject.transform.position.y - p.y;

                float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

                Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

                // Rotate our velocity to match the direction between the two objects
                float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (p.x > enemyLightObject.transform.position.x ? 1 : -1);
                Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

                // Fire!
                enemyLightObject.GetComponent<Rigidbody>().velocity = finalVelocity;
            }
        }
    }

    public override void StateEnd() {

    }
}
