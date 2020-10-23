using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase<GamePanel>
{
    [Header("Player Info Group")]
    [SerializeField] private Text _roundText;
    [SerializeField] private Text _gameStateText;
    [SerializeField] private Text _chessCountText;
    [SerializeField] private Text _goldText;
    [SerializeField] private Text _nicknameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Image _expImage;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Text _hpText;

    [Header("Selected Unit Info")]
    [SerializeField] private GameObject _selectedUnitInfo;
    [SerializeField] private Text _unitNameText;
    [SerializeField] private Text _unitClassText;
    [SerializeField] private Text _unitSpeciesText;
    [SerializeField] private Text _unitStarText;
    [SerializeField] private SkillIcon _unitSkillIcon;
    [SerializeField] private Image _unitSkillCoolTimeImage;
    [SerializeField] private Text _unitSkillCoolTimeText;
    [SerializeField] private Slider _unitHpSlider;
    [SerializeField] private Slider _unitMpSlider;
    [SerializeField] private Text _unitHpText;
    [SerializeField] private Text _unitMpText;
    [SerializeField] private Text _unitDamageTypeText;
    [SerializeField] private Text _unitAttackDamageText;
    [SerializeField] private Text _unitSkillDamageText;
    [SerializeField] private Text _unitDeffensePowerText;
    [SerializeField] private Text _unitMagicResistPowerText;

    [Header("Synergy Info")]
    [SerializeField] private Transform _synergyItemRoot;
    [SerializeField] private SynergyItem _synergyItem;

    private Unit _selectedUnit;

    public override void Init()
    {
        GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;
        Player.instance.OnLevelChanged += Instance_OnLevelChanged;
        Player.instance.OnExpChanged += Instance_OnExpChanged;
        Player.instance.OnGoldChanged += Instance_OnGoldChanged;
        Player.instance.OnHPChanged += Instance_OnHPChanged;
        FieldManager.instance.OnFieldChanged += Instance_OnFieldChanged;
    }
    private void Start()
    {
        _nicknameText.text = Player.instance.nickname;
        ShowUnitInfo(null);
    }

    private void Instance_OnFieldChanged(object sender, System.EventArgs e)
    {
        _chessCountText.text = $"{FieldManager.instance.GetCountOfChessOnMyField()}/{Player.instance.level}";

        //시너지 표시 업데이트
        SynergyItem[] items = _synergyItemRoot.GetComponentsInChildren<SynergyItem>();
        for (int i = 0; i < items.Length; i++)
            Destroy(items[i].gameObject);

        Dictionary<int, int> unitsOnField = FieldManager.instance.unitsOnField;
        Dictionary<Clas, int> classSynergy = new Dictionary<Clas, int>();
        Dictionary<Species, int> speciesSynergy = new Dictionary<Species, int>();
        foreach(var pair in unitsOnField)
        {
            if (pair.Value <= 0) continue;
            var data = TableData.instance.GetUnitTableData(pair.Key);
            if (classSynergy.ContainsKey(data.CLAS)) classSynergy[data.CLAS]++;
            else classSynergy.Add(data.CLAS, 1);
            if (speciesSynergy.ContainsKey(data.SPECIES)) speciesSynergy[data.SPECIES]++;
            else speciesSynergy.Add(data.SPECIES, 1);
        }
        foreach(var pair in classSynergy)
        {
            SynergyItem item = Instantiate(_synergyItem, _synergyItemRoot);
            item.SetUp(pair.Key, pair.Value);
        }
        foreach (var pair in speciesSynergy)
        {
            SynergyItem item = Instantiate(_synergyItem, _synergyItemRoot);
            item.SetUp(pair.Key, pair.Value);
        }
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        _gameStateText.text = GameManager.instance.gameState.ToString().ToUpper();
        _roundText.text = $"Round { GameManager.instance.round}";
    }

    private void Instance_OnHPChanged(object sender, System.EventArgs e)
    {
        _hpSlider.value = Player.instance.hp;
        _hpText.text = $"{Player.instance.hp} / 100";
    }

    private void Instance_OnGoldChanged(object sender, System.EventArgs e)
    {
        _goldText.text = Player.instance.gold.ToString();
    }

    private void Instance_OnExpChanged(object sender, System.EventArgs e)
    {
        _expImage.fillAmount = Player.instance.exp / (float)100;
    }

    private void Instance_OnLevelChanged(object sender, System.EventArgs e)
    {
        _levelText.text = Player.instance.level.ToString();
        _chessCountText.text = $"{FieldManager.instance.GetCountOfChessOnMyField()}/{Player.instance.level}";
    }

    public void ShowUnitInfo(Unit unit)
    {
        if(unit == null)
        {
            _selectedUnit = null;
            _selectedUnitInfo.SetActive(false);
            return;
        }

        _selectedUnit = unit;
        _selectedUnitInfo.SetActive(true);
        _unitNameText.text = _selectedUnit.Data.Name;
        _unitClassText.text = _selectedUnit.Data.CLAS.ToName();
        _unitSpeciesText.text = _selectedUnit.Data.SPECIES.ToName();
        _unitStarText.text = $"{_selectedUnit.star}성";
        _unitSkillIcon.SetUp(_selectedUnit);
        _unitDamageTypeText.text = $"피해 유형: {_selectedUnit.Data.DAMAGETYPE.ToName()}";
        _unitAttackDamageText.text = $"공격 데미지: {_selectedUnit.curAttackDamage}";
        _unitSkillDamageText.text = $"스킬 데미지: {_selectedUnit.curSkillDamage}";
        _unitDeffensePowerText.text = $"방어력: {_selectedUnit.curDeffensePower}";
        _unitMagicResistPowerText.text = $"마법 저항력: {_selectedUnit.curMagicResistPower}";
    }

    private void Update()
    {
        if (_selectedUnit != null)
        {
            _unitHpSlider.value = _selectedUnit.curHp / _selectedUnit.curFullHp;
            _unitMpSlider.value = _selectedUnit.curMp / _selectedUnit.curFullMp;
            _unitHpText.text = $"{(int)_selectedUnit.curHp} / {_selectedUnit.curFullHp}";
            _unitMpText.text = $"{(int)_selectedUnit.curMp} / {_selectedUnit.curFullMp}";
            _unitSkillCoolTimeImage.fillAmount = _selectedUnit.curSkillCoolTime / _selectedUnit.Data.Skillcooltime;
            _unitSkillCoolTimeText.gameObject.SetActive(_selectedUnit.curSkillCoolTime > 0);
            _unitSkillCoolTimeText.text = _selectedUnit.curSkillCoolTime.ToString("0.0");
        }
    }
}