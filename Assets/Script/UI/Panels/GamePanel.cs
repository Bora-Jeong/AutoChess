using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    [SerializeField] private Image _hpImage;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _expText;

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
    [SerializeField] private Text _unitAttackPowerText;
    [SerializeField] private Text _unitMagicPowerText;
    [SerializeField] private Text _unitDeffensePowerText;
    [SerializeField] private Text _unitMagicResistPowerText;
    [SerializeField] private Text _synergyHpText;
    [SerializeField] private Text _synergyAttackPowerText;
    [SerializeField] private Text _synergyMagicPowerText;
    [SerializeField] private Text _synergyDeffensePowerText;
    [SerializeField] private Text _synergyMagicResistPowerText;

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
        foreach (var pair in FieldManager.instance.curClassSynergy)
        {
            SynergyItem item = Instantiate(_synergyItem, _synergyItemRoot);
            item.SetUp(pair.Key, pair.Value);
        }
        foreach (var pair in FieldManager.instance.curSpeciesSynergy)
        {
            SynergyItem item = Instantiate(_synergyItem, _synergyItemRoot);
            item.SetUp(pair.Key, pair.Value);
        }
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        _gameStateText.text = GameManager.instance.gameState.ToString().ToUpper();
        _roundText.text = $"Round { GameManager.instance.round}";

        if (GameManager.instance.gameState == GameState.Battle || GameManager.instance.gameState == GameState.Prepare)
            ShowUnitInfo(_selectedUnit);
    }

    private void Instance_OnHPChanged(object sender, System.EventArgs e)
    {
        _hpImage.fillAmount = Player.instance.hp / 100;
        _hpText.text = $"{Player.instance.hp} / 100";
    }

    private void Instance_OnGoldChanged(object sender, System.EventArgs e)
    {
        _goldText.text = Player.instance.gold.ToString();
    }

    private void Instance_OnExpChanged(object sender, System.EventArgs e)
    {
        if (Player.instance.level < Constants.playerMaxLevel)
            _expText.text = $"{Player.instance.exp}/{Constants.requiredExpToLevelUp[Player.instance.level]}";
    }

    private void Instance_OnLevelChanged(object sender, System.EventArgs e)
    {
        _levelText.text = $"Lv.{Player.instance.level}";
        _chessCountText.text = $"{FieldManager.instance.GetCountOfChessOnMyField()}/{Player.instance.level}";
        if (Player.instance.level == Constants.playerMaxLevel)
            _expText.text = string.Empty;
    }

    public void OnBuyExpButton()
    {
        ShopPanel.instance.BuyExp();
    }

    public void ShowUnitInfo(Unit unit)
    {
        if (unit == null)
        {
            _selectedUnit = null;
            _selectedUnitInfo.SetActive(false);
            return;
        }

        _selectedUnit = unit;
        _selectedUnitInfo.SetActive(true);
        _unitNameText.text = _selectedUnit.Data.Name;
        _unitNameText.color = _selectedUnit.Data.Gold.GoldColor();
        _unitClassText.text = _selectedUnit.Data.CLAS.ToName();
        _unitSpeciesText.text = _selectedUnit.Data.SPECIES.ToName();
        _unitStarText.text = $"{_selectedUnit.star}성";
        _unitSkillIcon.SetUp(_selectedUnit);
        _unitDamageTypeText.text = $"피해 유형: {_selectedUnit.Data.DAMAGETYPE.ToName()}";
        _unitAttackPowerText.text = $"공격력: {_selectedUnit.curAttackPower}";
        _unitMagicPowerText.text = $"마법 주문력: {_selectedUnit.curMagicPower}";
        _unitDeffensePowerText.text = $"방어력: {_selectedUnit.curDeffensePower}";
        _unitMagicResistPowerText.text = $"마법 저항력: {_selectedUnit.curMagicResistPower}";
        SetSynergyText(_synergyHpText, _selectedUnit.synergyHp);
        SetSynergyText(_synergyAttackPowerText, _selectedUnit.synergyAttackPower);
        SetSynergyText(_synergyMagicPowerText, _selectedUnit.synergyMagicPower);
        SetSynergyText(_synergyDeffensePowerText, _selectedUnit.synergyDeffensePower);
        SetSynergyText(_synergyMagicResistPowerText, _selectedUnit.synergyMagicResistPower);
    }

    private void SetSynergyText(Text text, float num)
    {
        bool state = GameManager.instance.gameState == GameState.Battle || GameManager.instance.gameState == GameState.Result;
        text.gameObject.SetActive(state && num != 0);
        if (state && num != 0)
            text.text = num > 0 ? $"+{num}" : num.ToString("0.0");
    }

    private void Update()
    {
        if (_selectedUnit != null)
        {
            _unitHpSlider.value = _selectedUnit.curHp / _selectedUnit.fullHpOnBattle;
            _unitMpSlider.value = _selectedUnit.curMp / _selectedUnit.curFullMp;
            _unitHpText.text = $"{(int)_selectedUnit.curHp} / {_selectedUnit.curFullHp}";
            _unitMpText.text = $"{(int)_selectedUnit.curMp} / {_selectedUnit.curFullMp}";
            _unitSkillCoolTimeImage.fillAmount = _selectedUnit.curSkillCoolTime / _selectedUnit.Data.Skillcooltime;
            _unitSkillCoolTimeText.gameObject.SetActive(_selectedUnit.curSkillCoolTime > 0);
            _unitSkillCoolTimeText.text = _selectedUnit.curSkillCoolTime.ToString("0.0");
        }
    }
}