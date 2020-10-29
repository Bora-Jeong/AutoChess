using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private Unit _owner;
    private int _findTargetTryCount;

    public IdleState(Unit unit)
    {
        _owner = unit;
    }
    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Idle);
        _findTargetTryCount = 0;
    }

    public void Stay()
    {
        if(_owner.target == null || _owner.target.isDead) // 공격 대상이 없거나 공격 대상이 죽었으면 새로운 공격 대상 찾기
            FindTarget();
    }

    public void Exit()
    {
    }

    public void FindTarget()
    {
        _findTargetTryCount++;
        Collider[] colliders = Physics.OverlapSphere(_owner.transform.position, _owner.findTargetRange, 1 << LayerMask.NameToLayer("Unit"));
        if (colliders.Length <= 0) return; // 범위안에 아무도 없음
        Unit nearestTarget = null; // 가장 가까운 적을 저장하기 위한 변수
        float minDist = Mathf.Infinity; // 가장 가까운 적과의 거리를 저장하기 위한 변수
        for (int i = 0; i < colliders.Length; i++) // 콜라이더내에서 가장 가까운 적 검색
        {
            Unit temp = colliders[i].GetComponentInParent<Unit>();
            if(temp.onCell.type != Cell.Type.Inventory && !temp.isDead && _owner.isCreep != temp.isCreep) // 죽지 않은 상대편 유닛이면
            {
                float dist = Vector3.Distance(_owner.transform.position, temp.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestTarget = temp;
                }
            }
        }
        if (nearestTarget != null)
        {
            _owner.target = nearestTarget;  // 가장 가까운 적으로 타겟 설정
            _owner.state = Unit.State.Move; // 타겟을 향해 이동
            _findTargetTryCount = 0;
        }
        else if(_findTargetTryCount > Time.deltaTime * 2) // 한동안 적을 못찾으면 라운드가 끝났는지 확인
            FieldManager.instance.ReportRoundFinish();
    }
}
