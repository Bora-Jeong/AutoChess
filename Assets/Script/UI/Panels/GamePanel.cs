using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase<GamePanel>
{
    [Header("Player Info Group")]
    [SerializeField] private Text _roundText;
    [SerializeField] private Text _mobNameText;
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

    private Unit _selectedUnit;

    public override void Init()
    {
        GameManager.instance.OnRoundChanged += Instance_OnRoundChanged;
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
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        _gameStateText.text = GameManager.instance.gameState.ToString().ToUpper();
    }

    private void Instance_OnHPChanged(object sender, System.EventArgs e)
    {
        _hpSlider.value = Player.instance.hp;
        _hpText.text = $"{Player.instance.hp} / 100";
    }

    private void Instance_OnRoundChanged(object sender, System.EventArgs e)
    {
        _roundText.text = $"Round { GameManager.instance.round}";
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
    }

    private void Update()
    {
        if (_selectedUnit != null)
        {
            _unitHpSlider.value = _selectedUnit.curHp / _selectedUnit.Data.Hp;
            _unitMpSlider.value = _selectedUnit.curMp;
            _unitHpText.text = $"{(int)_selectedUnit.curHp} / {_selectedUnit.Data.Hp}";
            _unitMpText.text = $"{(int)_selectedUnit.curMp} / 100";
            _unitSkillCoolTimeImage.fillAmount = _selectedUnit.curSkillCoolTime / _selectedUnit.Data.Skillcooltime;
            _unitSkillCoolTimeText.gameObject.SetActive(_selectedUnit.curSkillCoolTime > 0);
            _unitSkillCoolTimeText.text = _selectedUnit.curSkillCoolTime.ToString("0.0");
        }
    }
}