using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State {

    private WispAI Wisp;
    private Transform transform;
    private float timer = 0.0f;

    public FleeState(WispAI wisp, Transform transform, float time) {
        Wisp = wisp;
        this.transform = transform;
        this.timer = time;
    }

    public override void StateBehaviour() {
        if (timer <= 0.0f) {
            Wisp.transform.rotation = Quaternion.LookRotation(-Wisp.transform.forward, Vector3.up);
        }

        Vector3 targetDir = Wisp.transform.position - new Vector3(transform.position.x, Wisp.transform.position.y, transform.position.z);
        Vector3 newDir = Vector3.RotateTowards(Wisp.transform.forward, targetDir, Time.deltaTime * Wisp.Speed, 0.0f);
        Wisp.transform.rotation = Quaternion.LookRotation(newDir);

        Wisp.transform.position = Wisp.transform.position + Wisp.transform.forward * Wisp.Speed * Time.deltaTime;
    }

    public override void StateEnd() {

    }

    public override void StateStart() {

    }
}
