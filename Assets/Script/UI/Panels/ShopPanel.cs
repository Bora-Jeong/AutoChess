﻿using QFSW.MOP2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : PanelBase<ShopPanel>
{
    [SerializeField]
    private Transform[] _renderPositions;
    [SerializeField]
    private ProductItem[] _productItems;
    [SerializeField]
    private Toggle _lockToggle;
    [SerializeField]
    private Text[] _spawPercentTexts;

    private Dictionary<int, List<int>> _unitGoldPoolDic; // 키 : 플레이어 레벨
    private Dictionary<int, List<int>> _unitIDDic; // 골드별 유닛 아이디

    public override void Init()
    {
        InitializeLists();
        _lockToggle.isOn = false;
        Player.instance.OnLevelChanged += Instance_OnLevelChanged;
    }

    private void Instance_OnLevelChanged(object sender, System.EventArgs e)
    {
        int start = (Player.instance.level - 1) * Constants.unitMaxGold;
        for (int i = 0; i < _spawPercentTexts.Length; i++)
        {
            _spawPercentTexts[i].text = $"{Constants.unitSpawnPercentInShop[start + i]}%";
            _spawPercentTexts[i].color = (i+1).GoldColor();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Shuffle(true);
    }

    public void Shuffle(bool pay = false)
    {
        if (pay && Player.instance.gold < Constants.requiredGoldToShuffleShop) return; // 돈 부족
        if (_lockToggle.isOn) return; // 잠금

        int level = Player.instance.level;
        List<int> randomPool = _unitGoldPoolDic[level-1];  // 플레이어 레벨의 유닛 확률 셋
        for(int i = 0; i < _renderPositions.Length; i++)
        {
            int unitGold = randomPool[Random.Range(0, randomPool.Count)]; // 확률에 따른 유닛 골드 뽑기
            List<int> unitIDPool = _unitIDDic[unitGold];  // 해당 골드의 유닛 풀
            int unitID = unitIDPool[Random.Range(0, unitIDPool.Count)]; // 유닛 랜덤 뽑기
            Unit unit = ObjectPoolManager.instance.GetUnit(unitID);
            unit.transform.SetParent(_renderPositions[i]);
            unit.transform.localPosition = Vector3.zero;
            unit.transform.localEulerAngles = Vector3.zero;
            _productItems[i].SetUp(unit);
        }

        if(pay) Player.instance.gold -= Constants.requiredGoldToShuffleShop;
    }

    public bool BuyUnit(Unit unit)
    {
        if (Player.instance.gold < unit.Data.Gold || InventoryManager.instance.IsFull()) return false;
        InventoryManager.instance.BuyUnit(unit.Data.Unitid);
        Player.instance.gold -= unit.Data.Gold;
        return true;
    }

    public void BuyExp()
    {
        if (Player.instance.gold < Constants.requiredGoldToBuyExp || Player.instance.level >= Constants.playerMaxLevel) return;
        Player.instance.gold -= Constants.requiredGoldToBuyExp;
        Player.instance.exp += Constants.amountOfExpBounght;
    }


    public void OnExitButtonClick()
    {
        Hide();
    }

    private void InitializeLists()
    {
        _unitGoldPoolDic = new Dictionary<int, List<int>>();
        for(int i = 0; i < Constants.playerMaxLevel; i++)
        {
            List<int> temp = new List<int>();
            for (int j = i * Constants.unitMaxGold; j < i * Constants.unitMaxGold + 5; j++)
                for (int k = 0; k < Constants.unitSpawnPercentInShop[j]; k++)
                    temp.Add(j % Constants.unitMaxGold);
            _unitGoldPoolDic.Add(i, temp);
        }

        _unitIDDic = new Dictionary<int, List<int>>();
        for (int i = 0; i < Constants.unitMaxGold; i++)
            _unitIDDic.Add(i, new List<int>());

        var unitDic = TableData.instance.unitTableDic;
        foreach(var par in unitDic)
            _unitIDDic[par.Value.Gold - 1].Add(par.Key);
    }

    public void OnShuffleButton()
    {
        Shuffle(true);
    }
}
