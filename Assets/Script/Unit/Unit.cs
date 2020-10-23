using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
    [HideInInspector]
    public float findTargetRange;  // 범위 안에 있는 적 탐색

    // 포물선 이동
    private readonly float _firingAngle = 45.0f;
    private readonly float _gravity = 20f;

    private Animator _animator;
    private Outline _outline;
    private IState[] _IStates;  // FSM 인터페이스 저장

    public float curHp;
    public float curMp;
    public float curSkillCoolTime;

    public float curFullHp { get; private set; }
    public float curFullMp { get; private set; }
    public float curAttackDamage { get; private set; }
    public float curSkillDamage { get; private set; }
    public float curDeffensePower { get; private set; }
    public float curMagicResistPower { get; private set; }
    public int curGold { get; private set; }

    public Cell onCell; // 현재 있는 셀 
    public Sprite skillIcon;

    public bool isCreep;

    public Unit target; // 공격 대상
    public int unitID => _unitID;
    public bool isDead => state == State.Dead;

    public UnitTableData Data { get; private set; }

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
            float scalePlus = 1 + Constants.unitScaleIncreaseAmount * ( _star - 1);
            transform.localScale = Vector3.one * Data.Scale * scalePlus; // 크기 증가
            curGold = Data.Gold * _star; // 골드 증가
            curFullHp = Data.Hp * Mathf.Pow(4, _star - 1); // 최대 체력 증가
            curHp = curFullHp;
            curFullMp = 100 * star; // 최대 마나 증가
            curAttackDamage = Data.Attackdamage * Mathf.Pow(4, _star - 1); // 공격 데미지 증가
            curSkillDamage = Data.Skilldamage * Mathf.Pow(4, _star - 1); // 스킬 데미지 증가
            curDeffensePower = Data.Deffensepower *  Mathf.Pow(2, _star - 1); // 방어력 증가
            curMagicResistPower = Data.Magicresistpower *  Mathf.Pow(2, _star - 1); // 마법 저항력 증가
        }
    }

    private void Awake()
    {
        _IStates = new IState[System.Enum.GetValues(typeof(State)).Length];
        _IStates[(int)State.Idle] = new IdleState(this);
        _IStates[(int)State.Walk] = new WalkState(this);
        _IStates[(int)State.Attack] = new AttackState(this);
        _IStates[(int)State.Dead] = new DeadState(this);

        Data = TableData.instance.GetUnitTableData(_unitID);
        GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;

        _animator = GetComponent<Animator>();
        skillIcon = GetSkillIcon(_unitID);
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.instance.gameState == GameManager.GameState.Prepare)
            ResetStateValues();
    }

    private void OnEnable()
    {
        star = 1;
        ResetStateValues();
    }

    private void ResetStateValues()
    {
        curHp = curFullHp;
        curMp = 0;
        state = State.Idle;
        curSkillCoolTime = 0;
    }

    private void OnDisable()
    {
        if (_outline != null) _outline.enabled = false;
        onCell = null;
        target = null;
        isCreep = false;
    }

    private void Update()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Battle || onCell == null || onCell.type == Cell.Type.Inventory) return;

        _IStates[(int)_state].Stay();

        findTargetRange += Time.deltaTime;
        findTargetRange = Mathf.Clamp(findTargetRange, 10f, 500f);
        curSkillCoolTime = Mathf.Clamp(curSkillCoolTime - Time.deltaTime, 0, Data.Skillcooltime);
    }

    public void TakeDamage(DamageType damageType, float damage)
    {
        float actualDamage = damageType == DamageType.Physics ? damage / curDeffensePower : damage / curMagicResistPower;

        curHp = Mathf.Clamp(curHp - actualDamage, 0, curFullHp);
        curMp = Mathf.Clamp(curMp + actualDamage, 0, curFullMp);

        if (curHp <= 0)
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
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * _firingAngle * Mathf.Deg2Rad) / _gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(_firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(_firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        Quaternion startRot = transform.rotation;
        transform.rotation = Quaternion.LookRotation(dest.position - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (_gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        transform.rotation = startRot;

        while(transform.rotation != startRot)
        {
            transform.Rotate(startRot.eulerAngles * Time.deltaTime);
        }
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

    public static Sprite GetSkillIcon(int unitID)
    {
        return Resources.Load<Sprite>($"SkillIcon/{unitID}");
    }
}
