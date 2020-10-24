using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private Unit _owner;
    private float _stunTimer;
    private StaticText _stunText;

    public StunState(Unit unit)
    {
        _owner = unit;
    }

    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Stun);
        _stunTimer = Constants.unitStunTime;
        if(_stunText == null)
            _stunText = StaticText.Create(_owner, "stun");
    }

    public void Stay()
    {
        _stunTimer -= Time.deltaTime;
        if (_stunTimer <= 0)
            _owner.state = Unit.State.Idle;
    }

    public void Exit()
    {
        if(_stunText != null)
        {
            ObjectPoolManager.instance.Release(_stunText.gameObject, ObjectPoolManager.PoolObj.StaticText);
            _stunText = null;
        }
    }
}
