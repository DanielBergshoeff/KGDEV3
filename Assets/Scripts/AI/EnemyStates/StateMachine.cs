using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {

    public State currentState;

    public void ExecuteState() {
        if (currentState != null)
            currentState.StateBehaviour();
    }

    public void SwitchState(State newState) {
        if (currentState != null)
            currentState.StateEnd();

        currentState = newState;
        currentState.StateStart();
    }
}
