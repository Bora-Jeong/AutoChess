using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private Unit _owner;
    public DeadState(Unit unit)
    {
        _owner = unit;
    }
    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Dead);
    }
    public void Stay()
    {

    }
    public void Exit()
    {
    }
}
