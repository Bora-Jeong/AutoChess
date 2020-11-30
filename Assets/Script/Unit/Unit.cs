using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public interface IState
{
    void Enter();
    void Stay();

    void Fixed();
    void Exit();
}

public class Unit : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Attack,
        Stun,
        Dead
    }

    public enum Weapon
    {
        Fist,
        Sword,
        Gun,
        None
    }

    [SerializeField]
    private string _idleAnimName = "Idle01";
    [SerializeField]
    private string _walkAnimName = "Run";
    [SerializeField]
    private string _stunAnimName = "Stun";
    [SerializeField]
    private string _deadAnimName = "Die";
    [SerializeField]
    private Weapon _weapon;
    [HideInInspector]
    public float findTargetRange;  // 범위 안에 있는 적 탐색

    // 포물선 이동
    private readonly float _firingAngle = 45.0f;
    private readonly float _gravity = 20f;

    private Animator _animator;
    private Outline _outline;
    private IState[] _IStates;  // State 인터페이스 저장
    private Rigidbody _rigidbody;

    public float curHp;
    public float curMp;
    public float curSkillCoolTime;

    public float curFullHp { get; set; }
    public float curFullMp { get; set; }
    public float curAttackPower { get; set; }
    public float curMagicPower { get; set; }
    public float curDeffensePower { get; set; }
    public float curMagicResistPower { get; set; }
    public float curAttackTerm { get; set; }
    public float curMoveSpeed { get; set; }

    public float synergyHp;
    public float synergyAttackPower;
    public float synergyMagicPower;
    public float synergyDeffensePower;
    public float synergyMagicResistPower;
    public float synergyEvasion; // 상대방의 공격을 회피할 확률
    public float synergyMakeSilent; // 상대방을 공격할 때 침묵시킬 확률

    public static bool dragonSynergy; // [용]
    public static float knightSynergy; // [기사] 쉴드 발동 확률
    public static float assassinSynergyPercent; // [암살자] 치명타 발생 확률
    public static float assassinSynergyCritical; // [암살자] 치명타 몇 배

    private float _kinghtSynergyTimer;
    private bool _isInvincibility; // 무적

    private Vector3 _prevPos;
    private Quaternion _prevRot;

    // 시너지가 적용된 전투중 스탯
    public float fullHpOnBattle => Mathf.Max(curFullHp + synergyHp, 0);
    public float attackPowerOnBattle => Mathf.Max(curAttackPower + synergyAttackPower, 0);
    public float magicPowerOnBattle => Mathf.Max(curMagicPower + synergyMagicPower, 0);
    public float deffensePowerOnBattle => Mathf.Max(curDeffensePower + synergyDeffensePower, 0);
    public float magicResistPowerOnBattle => Mathf.Max(curMagicResistPower + synergyMagicResistPower, 0);
    public int curGold { get; private set; }

    public Weapon weapon => _weapon;

    public Cell onCell; // 현재 있는 셀 
    public Sprite skillIcon;
    public bool isCreep;

    public Unit target; // 공격 대상
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
            curFullMp = 100 * _star; // 최대 마나 증가
            curAttackPower = Data.Attackpower * Mathf.Pow(4, _star - 1); // 공격력 증가
            curMagicPower = Data.Magicpower * Mathf.Pow(4, _star - 1); // 마법 주문력 증가
            curDeffensePower = Data.Deffensepower *  Mathf.Pow(2, _star - 1); // 방어력 증가
            curMagicResistPower = Data.Magicresistpower *  Mathf.Pow(2, _star - 1); // 마법 저항력 증가
        }
    }

    private void Awake()
    {
        _IStates = new IState[System.Enum.GetValues(typeof(State)).Length];
        _IStates[(int)State.Idle] = new IdleState(this);
        _IStates[(int)State.Move] = new MoveState(this);
        _IStates[(int)State.Attack] = new AttackState(this);
        _IStates[(int)State.Stun] = new StunState(this);
        _IStates[(int)State.Dead] = new DeadState(this);

        string[] result =  gameObject.name.Split('_', '(');
        if(int.TryParse(result[1], out int unitID))
        {
            Data = TableData.instance.GetUnitTableData(unitID);
            GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;

            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            skillIcon = GetSkillIcon(unitID);
        }
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        GameState gameState = GameManager.instance.gameState;
        if (gameState == GameState.Prepare)
        {
            if(onCell != null && onCell.type != Cell.Type.Inventory)
            {
                _rigidbody.velocity = Vector3.zero;
                transform.position = _prevPos;
                transform.rotation = _prevRot;
                ResetState();
            }
        }
        else if(gameState == GameState.Battle)
        {
            if (onCell != null && onCell.type != Cell.Type.Inventory)
            {
                _prevPos = transform.position;
                _prevRot = transform.rotation;
                curHp = fullHpOnBattle;
                if (dragonSynergy && Data.CLAS == Clas.Dragon)
                    curMp = curFullMp;
                if (knightSynergy > 0 && Data.SPECIES == Species.Knight && onCell != null && onCell.type == Cell.Type.MyField)
                    _kinghtSynergyTimer = Constants.kinghtShieldGenTime;
            }
        }
        else if (gameState == GameState.Result)
        {
            if (!isDead)
                state = State.Idle;
        }
    }

    private void OnEnable()
    {
        star = 1;
        curAttackTerm = Data.Attackterm;
        curMoveSpeed = Data.Movespeed;
        ResetState();
    }

    private void ResetState() // 라운드가 새로 시작할 때 마다 호출
    {
        curHp = curFullHp;
        curMp = 0;
        state = State.Idle;
        curSkillCoolTime = 0;
        synergyHp = synergyAttackPower = synergyMagicPower = synergyDeffensePower =
            synergyMagicResistPower = synergyEvasion =  synergyMakeSilent = 0;
        _isInvincibility = false;
        findTargetRange = 10f;
        target = null;
        _rigidbody.velocity = Vector3.zero;
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
        if (GameManager.instance.gameState != GameState.Battle || onCell == null || onCell.type == Cell.Type.Inventory) return;

        _IStates[(int)_state].Stay();

        findTargetRange = Mathf.Clamp(findTargetRange + Time.deltaTime, 10f, 1000f);
        curSkillCoolTime = Mathf.Clamp(curSkillCoolTime - Time.deltaTime, 0, Data.Skillcooltime);

        if (_kinghtSynergyTimer > 0 && !isDead) // 기사 시너지 
            CheckKnightSynergy();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState != GameState.Battle || onCell == null || onCell.type == Cell.Type.Inventory) return;

        _IStates[(int)_state].Fixed();
    }

    private void CheckKnightSynergy()
    {
        _kinghtSynergyTimer -= Time.deltaTime;
        if (_kinghtSynergyTimer <= 0)
        {
            _isInvincibility = false;
            if(Random.Range(0, 100) < knightSynergy) // 시너지 발동
            {
                _isInvincibility = true;
                ObjectPoolManager.instance.LaunchFx(transform, ObjectPoolManager.FxType.Shield);
            }
            _kinghtSynergyTimer = Constants.kinghtShieldGenTime;
        }
    }

    public void TakeDamage(DamageType damageType, float damage, bool isCritical)
    {
        if (_isInvincibility) return; // 무적
        if(onCell.type == Cell.Type.MyField && synergyEvasion > 0 && Random.Range(0, 100) < synergyEvasion) // 회피
        {
            PopUpText.Create(this, "evasion");
            return;
        }

        float actualDamage = damageType == DamageType.Physics ? damage / deffensePowerOnBattle : damage / magicPowerOnBattle;

        curHp = Mathf.Clamp(curHp - actualDamage, 0, fullHpOnBattle);
        curMp = Mathf.Clamp(curMp + actualDamage/2, 0, curFullMp);

        PopUpText.Create(this, ((int)actualDamage).ToString(), isCritical);

        if (DevCheat.instance.UNDEAD) return;         // 개발용

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

            case State.Move:
                _animator.Play(_walkAnimName);
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
