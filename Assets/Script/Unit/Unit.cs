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

    private Animator _animator;
    private float _hp;
    public float mp;
    private IState[] _IStates;  // FSM 인터페이스 저장
    public int unitID => _unitID;
    public bool isDead => state == State.Dead;
    public float findTargetRange { get; private set; }   // 범위 안에 있는 적 탐색
    public float attackTargetRange;  // 범위 안에 있는 적 공격
    public float attackTerm = 1f; // 공격 텀
    public float attackPower = 10f; // 공격력
    public float moveSpeed = 3f;
    public bool isPlayerUnit;

    public Unit target; // 공격 대상

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
    private void Awake()
    {
        _IStates = new IState[System.Enum.GetValues(typeof(State)).Length];
        _IStates[(int)State.Idle] = new IdleState(this);
        _IStates[(int)State.Walk] = new WalkState(this);
        _IStates[(int)State.Attack] = new AttackState(this);
        _IStates[(int)State.Dead] = new DeadState(this);

        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _hp = 100f;
        mp = 0f;
        state = State.Idle;
    }

    private void Update()
    {
        if (!GameManager.instance.isPlaying || isDead) return;

        _IStates[(int)_state].Stay();

        findTargetRange += Time.deltaTime;
        findTargetRange = Mathf.Clamp(findTargetRange, 10f, 500f);
    }

    public void TakeDamage(float damage)
    {
        _hp = Mathf.Clamp(_hp - 100, 0, 100);
        mp = Mathf.Clamp(mp + damage, 0, 100);

        if (_hp <= 0)
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
}
