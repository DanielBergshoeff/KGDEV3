using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : State {

    private WispAI Wisp;
    private Vector3 pos;
    private float timer = 0.0f;

    public RoamState(WispAI wisp) {
        Wisp = wisp;
    }

    public override void StateBehaviour() {
        if (Wisp.neuralNetworkRoam != null) {
            timer += Time.deltaTime;
            if(timer >= 3.0f) {
                timer = 0.0f;

                float dist = Vector3.Distance(pos, Wisp.transform.position);
                if (dist < 1.5f)
                    Wisp.SelfDestroy(null);
                Wisp.neuralNetworkRoam.AddFitness(dist * Time.deltaTime);
                pos = Wisp.transform.position;
            }
        }

        Wisp.transform.position = Wisp.transform.position + Wisp.transform.forward * Wisp.Speed * Time.deltaTime;

        bool raycastforwardHit = false;
        RaycastHit raycastHitForward;
        if(Physics.SphereCast(Wisp.transform.position, 0.5f, Wisp.transform.forward, out raycastHitForward, Wisp.forwardRaycastDistance)) {
            raycastforwardHit = true;
        }
        

        RaycastHit raycastHitLeft;
        Vector3 left = Quaternion.AngleAxis(-60f, Vector3.up) * Wisp.transform.forward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, left, out raycastHitLeft, Wisp.forwardRaycastDistance);

        RaycastHit rayCastHitRight;
        Vector3 right = Quaternion.AngleAxis(60f, Vector3.up) * Wisp.transform.forward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, right, out rayCastHitRight, Wisp.forwardRaycastDistance);

        float[] inputs = new float[] {
            raycastHitForward.distance,
            raycastHitLeft.distance,
            rayCastHitRight.distance
        };

        float[] outputs = Wisp.neuralNetworkRoam.FeedForward(inputs);

        //Rotate left based on outputs 0
        if (outputs[0] > 0.0f)
            Wisp.transform.Rotate(0.0f, -10.0f, 0.0f);

        //Rotate right based on outputs 1
        if (outputs[1] > 0.0f)
            Wisp.transform.Rotate(0.0f, 10.0f, 0.0f);

        List<GameObject> onTimeOut = new List<GameObject>();
        for (int i = 0; i < Wisp.lightTimers.Count; i++) {
            onTimeOut.Add(Wisp.lightTimers[i].light);
        }
        GameObject light = LightManager.GetClosestLightInRange(Wisp.transform.position, onTimeOut);

        if (light != null) {
            Wisp.lightTimers.Add(new LightTimer(light, 30.0f));
            Wisp.stateMachine.SwitchState(new AttractState(Wisp, light.transform, light.GetComponent<Light>().range / 10.0f));
        }

        if (raycastforwardHit) {
            if (raycastHitForward.distance < 3.0f && raycastHitForward.collider.CompareTag("Enemy")) {
                Wisp.stateMachine.SwitchState(new FleeState(Wisp, raycastHitForward.collider.transform, 1.0f));
            }
        }
    }

    public override void StateEnd() {

    }

    public override void StateStart() {

    }
}
