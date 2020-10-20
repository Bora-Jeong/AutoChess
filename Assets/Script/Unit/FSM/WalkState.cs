﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : IState
{
    private Unit _owner;
    public WalkState(Unit unit)
    {
        _owner = unit;
    }
    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Walk);
    }
    public void Stay()
    {
        Unit target = _owner.target;
        if (target == null || target.isDead)
        {
            _owner.state = Unit.State.Idle;
            return;
        }

        _owner.transform.LookAt(target.transform);
        _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, target.transform.position, Time.deltaTime * _owner.moveSpeed);

        if (Vector3.Distance(_owner.transform.position, target.transform.position) <= _owner.attackTargetRange) // 공격 사거리 안에 들어오면 공격 시작
            _owner.state = Unit.State.Attack;
    }

    public void Exit()
    {

    }
}
