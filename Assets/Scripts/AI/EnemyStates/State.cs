using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {
    
    public abstract void StateStart();

    public abstract void StateBehaviour();

    public abstract void StateEnd();

}
