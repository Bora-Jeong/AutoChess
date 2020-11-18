using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    private Unit _owner;
    private Rigidbody _rigidbody;

    public MoveState(Unit unit)
    {
        _owner = unit;
        _rigidbody = unit.GetComponent<Rigidbody>();
    }

    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Move);
        _rigidbody.velocity = Vector3.zero;
    }

    public void Stay()
    {
        Unit target = _owner.target;
        if (target == null || target.isDead)
        {
            _owner.state = Unit.State.Idle;
            return;
        }
    }

    public void Fixed()
    {
        Unit target = _owner.target;
        Vector3 dir = (target.transform.position - _owner.transform.position).normalized;
        _owner.transform.LookAt(target.transform);
       // _rigidbody.AddForce(dir.normalized * _owner.Data.Movespeed, ForceMode.Acceleration);
        _rigidbody.MovePosition(_owner.transform.position + dir * Time.deltaTime * _owner.curMoveSpeed);

        if (Vector3.Distance(_owner.transform.position, target.transform.position) <= _owner.Data.Attackrange) // 공격 사거리 안에 들어오면
        {
            _owner.state = Unit.State.Attack;
        }


        //Vector3 pos = _owner.transform.position;
        //if (Physics.CapsuleCast(pos + Vector3.up, pos + Vector3.down, 100f, dir, out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("Unit")))
        //{
        //    Unit unit = hit.collider.GetComponent<Unit>();
        //    if(unit == target)
        //    {
        //        Debug.Log($"타겟 발견, 공격 개시");
        //        _owner.state = Unit.State.Attack;
        //    }
        //    else if (unit.isCreep != _owner.isCreep && target.onCell != null && target.onCell.type != Cell.Type.Inventory)
        //    {
        //        Debug.Log($"새로운 적 발견 {unit.gameObject.name}");
        //        _owner.target = unit;
        //    }
        //}
    }

    public void Exit()
    {
        _rigidbody.velocity = Vector3.zero;
    }
}
