using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : PanelBase<LobbyPanel>
{
    [SerializeField] private Text _nicknameText;

    [Header("Ranking Tab")]
    [SerializeField] private Transform _rankingItemRoot;
    [SerializeField] private RankingItem _rankingItem;

    [Header("Unit Tab")]
    [SerializeField] private Dropdown _classDropdown;
    [SerializeField] private Dropdown _speciesDropdown;
    [SerializeField] private Dropdown _goldDropdown;
    [SerializeField] private Transform _unitListItemRoot;
    [SerializeField] private UnitListItem _unitListItem;
    [SerializeField] private GameObject _selectedUnitInfo;
    [SerializeField] private Transform _renderPosition;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _damageTypeText;
    [SerializeField] private Text _attackPowerText;
    [SerializeField] private Text _magicPowerText;
    [SerializeField] private Text _deffensePowerText;
    [SerializeField] private Text _magicResistPowerText;
    [SerializeField] private Text _skillCoolTimeText;
    [SerializeField] private SkillIcon _skillIcon;

    private List<UnitListItem> _unitList;
    private Unit _selectedUnit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopUpPanel.instance.PopUpYesOrNo("게임을 종료하시겠습니까?", () =>
            {
                Application.Quit();
            }, null);
        }
    }

    public override void Init()
    {
        InitUnitTab();
    }

    public void RefreshRanking(HNetPacket Packet)
    {
        RankingItem[] items = _rankingItemRoot.GetComponentsInChildren<RankingItem>();
        for (int i = 0; i < items.Length; i++)
            Destroy(items[i].gameObject);

        int rank = 1;
        while (!Packet.Empty())
        {
            string nickname = "";
            int win = 0, lose = 0;
            float rate = 0f;
            Packet.Out(ref nickname, Encoding.Unicode);
            Packet.Out(ref win);
            Packet.Out(ref lose);
            Packet.Out(ref rate);
            RankingItem item = Instantiate(_rankingItem, _rankingItemRoot);
            item.SetUp(rank, nickname, win, lose, rate);
            rank++;
        }

        _nicknameText.text = PlayerPrefs.GetString("Nickname", "New Player");
    }

    private void InitUnitTab()
    {
        UnitListItem[] items = _unitListItemRoot.GetComponentsInChildren<UnitListItem>();
        for (int i = 0; i < items.Length; i++)
            Destroy(items[i].gameObject);

        var unitDic = TableData.instance.unitTableDic;
        _unitList = new List<UnitListItem>();
        foreach (var pair in unitDic)
        {
            UnitListItem item = Instantiate(_unitListItem, _unitListItemRoot);
            item.data = pair.Value;
            _unitList.Add(item);
        }

        List<string> clas = new List<string>();
        for (int i = 0; i < System.Enum.GetValues(typeof(Clas)).Length; i++)
            clas.Add(((Clas)i).ToName());
        _classDropdown.AddOptions(clas);

        List<string> species = new List<string>();
        for (int i = 0; i < System.Enum.GetValues(typeof(Species)).Length; i++)
            species.Add(((Species)i).ToName());
        _speciesDropdown.AddOptions(species);

        List<string> gold = new List<string>();
        for (int i = 1; i <= Constants.unitMaxGold; i++)
            gold.Add(i.ToString());
        _goldDropdown.AddOptions(gold);

        _selectedUnitInfo.SetActive(false);
    }

    public void OnUnitListItemClick(int unitID)
    {
        if(_selectedUnit != null)
            ObjectPoolManager.instance.Release(_selectedUnit);

        UIManager.instance.PlayButtonClickSfx();
        _selectedUnitInfo.SetActive(true);
        _selectedUnit = ObjectPoolManager.instance.GetUnit(unitID);
        _selectedUnit.transform.SetParent(_renderPosition);
        _selectedUnit.transform.localPosition = Vector3.zero;
        _selectedUnit.transform.localEulerAngles = Vector3.zero;
        _hpText.text = $"체력: {_selectedUnit.Data.Hp}";
        _damageTypeText.text = $"피해 유형{_selectedUnit.Data.DAMAGETYPE.ToName()}";
        _attackPowerText.text = $"공격력: {_selectedUnit.Data.Attackpower}";
        _magicPowerText.text = $"마법 주문력: {_selectedUnit.Data.Magicpower}";
        _deffensePowerText.text = $"방어력: {_selectedUnit.Data.Deffensepower}";
        _magicResistPowerText.text = $"마법 저항력: {_selectedUnit.Data.Magicresistpower}";
        _skillCoolTimeText.text = $"스킬 쿨타임: {_selectedUnit.Data.Skillcooltime}초";
        _skillIcon.SetUp(_selectedUnit);
    }

    public void OnDropDownChanged()
    {
        UIManager.instance.PlayButtonClickSfx();
        _selectedUnitInfo.SetActive(false);
        if (UnitListItem.SelectedItem != null)
            UnitListItem.SelectedItem.DeSelect();

        int clas = _classDropdown.value;
        int speciese = _speciesDropdown.value;
        int gold = _goldDropdown.value;
        for(int i = 0; i < _unitList.Count; i++)
        {
            _unitList[i].gameObject.SetActive((clas == 0 || (int)_unitList[i].data.CLAS == clas - 1) &&
                (speciese == 0 || (int)_unitList[i].data.SPECIES == speciese - 1) &&
                (gold == 0 || _unitList[i].data.Gold == gold));
        }

        StartListAnimation(_unitListItemRoot);
    }

    public override void Hide()
    {
        if (_selectedUnit != null)
        {
            ObjectPoolManager.instance.Release(_selectedUnit);
            _selectedUnit = null;
        }
        base.Hide();
    }

    public void OnStartButtonClick()
    {
        UIManager.instance.PlayButtonClickSfx();
        GameManager.instance.StartGame();
    }   

    public void OnRankingTabOn()
    {
        StartListAnimation(_rankingItemRoot);
    }

    public void OnUnitTabOn()
    {
        StartListAnimation(_unitListItemRoot);
    }
}