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

            _owner.target.TakeDamage(_owner.attackDamage);
            _owner.mp = Mathf.Clamp(_owner.mp + _owner.attackDamage, 0, 100);

            yield return new WaitForSeconds(_owner.attackTermTime - 0.3f);

            if (_owner.mp >= 100f) // 마나가 꽉 차면 스킬 시전
            {
                _animator.SetTrigger("Skill");
                Debug.Log($"## {_owner.gameObject.name} 스킬 시전");
                _owner.mp -= 100;

                yield return new WaitForSeconds(0.3f);

                _owner.target.TakeDamage(_owner.skillDamage);
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
