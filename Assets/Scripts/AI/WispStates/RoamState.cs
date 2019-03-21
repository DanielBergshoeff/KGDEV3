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
        if (Physics.OverlapSphere(Wisp.transform.position, 0.5f).Length > 0)
            Wisp.SelfDestroy();

        if (Wisp.neuralNetworkRoam != null) {
            //Wisp.neuralNetworkRoam.AddFitness(Time.deltaTime);
            timer += Time.deltaTime;
            if(timer >= 3.0f) {
                timer = 0.0f;

                Wisp.neuralNetworkRoam.AddFitness(Vector3.Distance(pos, Wisp.transform.position) * Time.deltaTime);
                pos = Wisp.transform.position;
            }
        }



        Wisp.transform.position = Wisp.transform.position + Wisp.transform.forward * Wisp.Speed * Time.deltaTime;
        
        RaycastHit raycastHitForward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, Wisp.transform.forward, out raycastHitForward, Wisp.forwardRaycastDistance);

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
        
    }

    public override void StateEnd() {

    }

    public override void StateStart() {

    }
}
