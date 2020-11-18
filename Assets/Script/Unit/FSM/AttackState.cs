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

        _owner.transform.LookAt(_owner.target.transform);
    }

    private IEnumerator AttackCoroutine()
    {
        while (!_owner.isDead && !_owner.target.isDead)
        {
            _animator.SetTrigger("Attack");
            if (_owner.weapon == Unit.Weapon.Gun)
                SoundManager.PlaySFX("gun_ready"); // 장전 효과음

            yield return new WaitForSeconds(0.3f);

            int random = Random.Range(1, 3);
            if (_owner.weapon == Unit.Weapon.Fist)
                SoundManager.PlaySFX($"punch{random}");
            else if (_owner.weapon == Unit.Weapon.Sword)
                SoundManager.PlaySFX($"sword{random}");
            else if (_owner.weapon == Unit.Weapon.Gun)
                SoundManager.PlaySFX("gun_shoot");


            float damage = _owner.Data.DAMAGETYPE == DamageType.Physics ? _owner.attackPowerOnBattle : _owner.magicPowerOnBattle;
            damage *= 30;

            bool critical = false;
            if (Unit.assassinSynergyPercent > 0 && _owner.Data.SPECIES == Species.Assassin && Random.Range(0, 100) < Unit.assassinSynergyPercent) // 암살자 치명타
            {
                damage *= Unit.assassinSynergyCritical;
                critical = true;
            }
                  
            _owner.target.TakeDamage( _owner.Data.DAMAGETYPE, damage, critical);
            _owner.curMp = Mathf.Clamp(_owner.curMp + damage/20, 0, _owner.curFullMp);

            if(_owner.synergyMakeSilent > 0 && Random.Range(0, 100) < _owner.synergyMakeSilent)   // 침묵시킴
            {
                _owner.target.state = Unit.State.Stun;
            }

            yield return new WaitForSeconds(_owner.curAttackTerm - 0.3f);

            if (_owner.curMp >= _owner.curFullMp && _owner.curSkillCoolTime <= 0) // 스킬 시전
            {
                _animator.SetTrigger("Skill");
                _owner.curMp -= _owner.curFullMp;
                _owner.curSkillCoolTime = _owner.Data.Skillcooltime;

                yield return new WaitForSeconds(0.3f);

                damage = _owner.Data.DAMAGETYPE == DamageType.Physics ? _owner.attackPowerOnBattle : _owner.magicPowerOnBattle;
                damage *= 50;
                _owner.target.TakeDamage(_owner.Data.DAMAGETYPE, damage, false);
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

    public void Fixed()
    {
    }
}
