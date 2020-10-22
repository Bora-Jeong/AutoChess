using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MonoBehaviour, IState
{
    private Unit _owner;
    private Animator _animator;
    private Coroutine _attackCoroutine;

    public AttackState(Unit unit)
    {
        _owner = unit;
        _animator = _owner.GetComponent<Animator>();
    }
    public void Enter()
    {
        _owner.PlayAnimation(Unit.State.Idle);
        _attackCoroutine = _owner.StartCoroutine(AttackCoroutine());
    }

    public void Stay()
    {
        if (_owner.target.isDead)
        {
            _owner.state = Unit.State.Idle;
            return;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (!_owner.isDead && !_owner.target.isDead)
        {
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.3f);

            _owner.target.TakeDamage(_owner.Data.Attackdamage);
            _owner.curMp = Mathf.Clamp(_owner.curMp + _owner.Data.Attackdamage, 0, 100);

            yield return new WaitForSeconds(_owner.Data.Attackterm - 0.3f);

            if (_owner.curMp >= 100f && _owner.curSkillCoolTime == 0) // 마나가 꽉 차면 스킬 시전
            {
                _animator.SetTrigger("Skill");
                _owner.curMp -= 100;
                _owner.curSkillCoolTime = _owner.Data.Skillcooltime;

                yield return new WaitForSeconds(0.3f);

                _owner.target.TakeDamage(_owner.Data.Skilldamage);
            }
        }
    }

    public void Exit()
    {
        if (_attackCoroutine != null)
        {
            _owner.StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }
}
