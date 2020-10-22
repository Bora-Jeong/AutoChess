using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Stay();
    void Exit();
}

public class Unit : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walk,
        Attack,
        Dead
    }

    [SerializeField]
    private int _unitID;
    [SerializeField]
    private string _idleAnimName = "Idle01";
    [SerializeField]
    private string _walkAnimName = "Run";
    [SerializeField]
    private string _attackAnimName = "Attack01";
    [SerializeField]
    private string _skillAnimName = "Skill01";
    [SerializeField]
    private string _deadAnimName = "Die";

    private readonly float firingAngle = 45.0f;
    private readonly float gravity = 12f;

    private Animator _animator;
    private Outline _outline;
    private IState[] _IStates;  // FSM 인터페이스 저장

    public string name;
    public float fullHp;
    public float hp;
    public float mp;
    public float findTargetRange;  // 범위 안에 있는 적 탐색
    public float attackTargetRange;  // 범위 안에 있는 적 공격
    public float attackTermTime = 1f; // 공격 텀
    public float attackDamage = 1f; // 공격 데미지
    public float skillDamage = 20f; // 스킬 데미지
    public float moveSpeed = 3f;
    public bool isPlayerUnit;
    public int gold;
    public float defaultScale = 1f;
    public Class clas;
    public Species species;
    public Sprite skillIcon;

    public Cell onCell; // 현재 있는 셀 
    public bool isInShop = false;

    public Unit target; // 공격 대상
    public int unitID => _unitID;
    public bool isDead => state == State.Dead;

    private State _state;
    public State state
    {
        get => _state;
        set
        {
            _IStates[(int)_state].Exit();
            _state = value;
            _IStates[(int)_state].Enter();
        }
    }

    private int _star;
    public int star
    {
        get => _star;
        set
        {
            _star = value;
            float increase = 1 + Constants.unitScaleIncreaseAmount * ( _star-1);
            transform.localScale = Vector3.one * defaultScale * increase;
            // 공격력 강해짐 체력 증가 등
        }
    }

    private void Awake()
    {
        _IStates = new IState[System.Enum.GetValues(typeof(State)).Length];
        _IStates[(int)State.Idle] = new IdleState(this);
        _IStates[(int)State.Walk] = new WalkState(this);
        _IStates[(int)State.Attack] = new AttackState(this);
        _IStates[(int)State.Dead] = new DeadState(this);

        _animator = GetComponent<Animator>();
        skillIcon = Resources.Load<Sprite>($"SkillIcon/skillIcon_{_unitID}");
    }

    private void OnEnable()
    {
        hp = fullHp;
        mp = 0f;
        state = State.Idle;
        star = 1;
    }

    private void Update()
    {
        if (!GameManager.instance.isPlaying || isDead || isInShop) return;

        _IStates[(int)_state].Stay();

        findTargetRange += Time.deltaTime;
        findTargetRange = Mathf.Clamp(findTargetRange, 10f, 500f);
    }

    public void TakeDamage(float damage)
    {
        hp = Mathf.Clamp(hp - damage, 0, fullHp);
        mp = Mathf.Clamp(mp + damage, 0, 100);

        if (hp <= 0)
            state = State.Dead;
    }

    public void PlayAnimation(State animState)
    {
        switch (animState)
        {
            case State.Idle:
                _animator.Play(_idleAnimName);
                break;

            case State.Walk:
                _animator.Play(_walkAnimName);
                break;

            case State.Attack:
                _animator.Play(_attackAnimName);
                break;

            case State.Dead:
                _animator.Play(_deadAnimName);
                break;
        }
    }

    public void MoveToCell(Cell cell)
    {
        StartCoroutine(MoveProjectile(cell.transform));
    }

    IEnumerator MoveProjectile(Transform dest)
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(0.2f);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, dest.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        Quaternion startRot = transform.rotation;
        transform.rotation = Quaternion.LookRotation(dest.position - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        transform.rotation = startRot;
    }

    public void Ouline(bool on)
    {
        if (_outline == null)
        {
            _outline = gameObject.AddComponent<Outline>();
            _outline.OutlineColor = Color.red;
            _outline.OutlineWidth = 5f;
        }
        _outline.enabled = on;
    }
}
