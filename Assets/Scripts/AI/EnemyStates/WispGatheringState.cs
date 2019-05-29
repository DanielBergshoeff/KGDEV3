using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispGatheringState : State {

    private EnemyAI EnemyAI;
    private GameObject lightToUse;

    public WispGatheringState(EnemyAI EnemyAI) {
        this.EnemyAI = EnemyAI;
    }

    public override void StateStart() {
        lightToUse = LightManager.GetClosestLightInRange(EnemyAI.transform.position, EnemyAI.lightsInUse, EnemyAI.gameObject);
        if (lightToUse != null) {
            EnemyAI.lightsInUse.Add(lightToUse);
        }
    }

    public override void StateBehaviour() {
        if (lightToUse == null)
            StateStart();
        else {
            WispAI bestWisp = null;
            float dist = float.PositiveInfinity;
            foreach(WispAI wisp in WispSpawner.AllWisps) {
                float wispToLightDist = Vector3.Distance(lightToUse.transform.position, wisp.transform.position);
                if (wispToLightDist <= 15.0f && wispToLightDist >= 3.0f && wispToLightDist < dist) {
                    bestWisp = wisp;
                    dist = wispToLightDist;
                }
            }

            if (bestWisp != null) {
                var heading = lightToUse.transform.position - bestWisp.transform.position;
                var direction = heading / heading.magnitude;
                Vector3 positionToMoveTo = lightToUse.transform.position - direction * 2;
                EnemyAI.SetTargetPosition(positionToMoveTo);
                EnemyAI.MoveAlongPath(EnemyAI.walkSpeed);
                if(dist < 5.5f) {
                    EnemyAI.GetComponent<Animator>().SetTrigger("Stick_Up");
                }
            }
        }
    }

    public override void StateEnd() {
        EnemyAI.lightsInUse.Remove(lightToUse);
        lightToUse = null;
    }
}
