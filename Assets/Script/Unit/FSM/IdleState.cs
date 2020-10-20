using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Unit _owner;
    public IdleState(Unit unit)
    {
        _owner = unit;
    }
    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Idle);
    }

    public void Stay()
    {
        if(_owner.target == null || _owner.target.isDead)
            FindTarget();
    }

    public void Exit()
    {
    }

    public void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(_owner.transform.position, _owner.findTargetRange, 1 << LayerMask.NameToLayer("Unit"));
        if (colliders.Length <= 0) return; // 타겟 범위안에 없음
        for(int i = 0; i < colliders.Length; i++)
        {
            Unit temp = colliders[i].GetComponentInParent<Unit>();
            if(!temp.isDead && _owner.isPlayerUnit != temp.isPlayerUnit) // 죽지 않은 상대편 유닛이면
            {
                _owner.target = temp;
                _owner.state = Unit.State.Walk;
                break;
            }
        }
    }
}
