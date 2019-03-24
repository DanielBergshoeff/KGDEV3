using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringState : State {

    private WispAI Wisp;

    private float timer = 0.0f;

    public GatheringState(WispAI wisp) {
        Wisp = wisp;
        timer = 0.0f;
    }

    public override void StateBehaviour() {
        timer += Time.deltaTime;

        Wisp.transform.position = Wisp.transform.position + Wisp.transform.forward * Wisp.Speed * Time.deltaTime;

        Collider[] colliders = Physics.OverlapSphere(Wisp.transform.position, 0.5f);
        if (colliders.Length > 0) {
            Wisp.SetFitnessAndDestroy(colliders);
        }

        RaycastHit raycastHitForward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, Wisp.transform.forward, out raycastHitForward, Wisp.forwardRaycastDistance);

        RaycastHit raycastHitLeft;
        Vector3 left = Quaternion.AngleAxis(-60f, Vector3.up) * Wisp.transform.forward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, left, out raycastHitLeft, Wisp.forwardRaycastDistance);

        RaycastHit rayCastHitRight;
        Vector3 right = Quaternion.AngleAxis(60f, Vector3.up) * Wisp.transform.forward;
        Physics.SphereCast(Wisp.transform.position, 0.5f, right, out rayCastHitRight, Wisp.forwardRaycastDistance);

        float[] inputs = new float[] {
            //Raycast hit distances
            raycastHitForward.distance,
            raycastHitLeft.distance,
            rayCastHitRight.distance,

            //Wisp position
            Wisp.transform.position.x,
            Wisp.transform.position.y,
            Wisp.transform.position.z,

            //Wisp rotation
            Wisp.transform.rotation.eulerAngles.x,
            Wisp.transform.rotation.eulerAngles.y,
            Wisp.transform.rotation.eulerAngles.z,

            //Target position
            Wisp.targetPosition.position.x,
            Wisp.targetPosition.position.y,
            Wisp.targetPosition.position.z,
        };

        float[] outputs = Wisp.neuralNetworkGathering.FeedForward(inputs);

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
