using QFSW.MOP2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : PanelBase<ShopPanel>
{
    [SerializeField]
    private Transform[] _renderPositions;
    [SerializeField]
    private ProductItem[] _productItems;

    private int[][] _unitSpawnPercent; // [플레이어레벨][유닛 코인]

    private Dictionary<int, List<int>> _unitGoldPoolDic;
    private Dictionary<int, List<int>> _unitIDDic; // 골드별 유닛 아이디

    public override void Init()
    {
        InitializeValues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Shuffle();
        }
    }

    public void Shuffle()
    {
        if (Player.instance.gold < Constants.goldToShuffleShop) return;

        int level = Player.instance.level;
        List<int> randomPool = _unitGoldPoolDic[level-1];  // 플레이어 레벨의 유닛 확률 셋
        for(int i = 0; i < _renderPositions.Length; i++)
        {
            int unitGold = randomPool[Random.Range(0, randomPool.Count)]; // 확률에 따른 유닛 골드 뽑기
            List<int> unitIDPool = _unitIDDic[unitGold];  // 해당 골드의 유닛 풀
            int unitID = unitIDPool[Random.Range(0, unitIDPool.Count)]; // 유닛 랜덤 뽑기
            Unit unit = ObjectPoolManager.instance.GetUnit(unitID);
            unit.isInShop = true;
            unit.transform.SetParent(_renderPositions[i]);
            unit.transform.localPosition = Vector3.zero;
            unit.transform.localEulerAngles = Vector3.zero;
            _productItems[i].SetUp(unit);
        }

        Player.instance.gold -= Constants.goldToShuffleShop;
    }

    public void BuyUnit(Unit unit)
    {
        if (Player.instance.gold < unit.gold || InventoryManager.instance.IsFull()) return;
        InventoryManager.instance.AddUnit(unit.unitID);
        Player.instance.gold -= unit.gold;
    }


    public void OnExitButtonClick()
    {
        Hide();
    }

    private void InitializeValues()
    {
        _unitSpawnPercent = new int[Constants.playerMaxLevel][];
        for (int i = 0; i < _unitSpawnPercent.Length; i++)
            _unitSpawnPercent[i] = new int[Constants.unitMaxGold];

        // 플레이어 1레벨
        _unitSpawnPercent[0][0] = 100;

        // 플레이어 2레벨
        _unitSpawnPercent[1][0] = 70;
        _unitSpawnPercent[1][1] = 30;

        // 플레이어 3레벨
        _unitSpawnPercent[2][0] = 60;
        _unitSpawnPercent[2][1] = 35;
        _unitSpawnPercent[2][2] = 5;

        // 플레이어 4레벨
        _unitSpawnPercent[3][0] = 50;
        _unitSpawnPercent[3][1] = 35;
        _unitSpawnPercent[3][2] = 15;

        // 플레이어 5레벨
        _unitSpawnPercent[4][0] = 40;
        _unitSpawnPercent[4][1] = 35;
        _unitSpawnPercent[4][2] = 23;
        _unitSpawnPercent[4][3] = 2;

        // 플레이어 6레벨
        _unitSpawnPercent[5][0] = 33;
        _unitSpawnPercent[5][1] = 30;
        _unitSpawnPercent[5][2] = 30;
        _unitSpawnPercent[5][3] = 7;

        // 플레이어 7레벨
        _unitSpawnPercent[6][0] = 30;
        _unitSpawnPercent[6][1] = 30;
        _unitSpawnPercent[6][2] = 30;
        _unitSpawnPercent[6][3] = 10;

        // 플레이어 8레벨
        _unitSpawnPercent[7][0] = 24;
        _unitSpawnPercent[7][1] = 30;
        _unitSpawnPercent[7][2] = 30;
        _unitSpawnPercent[7][3] = 15;
        _unitSpawnPercent[7][4] = 1;

        // 플레이어 9레벨
        _unitSpawnPercent[8][0] = 22;
        _unitSpawnPercent[8][1] = 30;
        _unitSpawnPercent[8][2] = 25;
        _unitSpawnPercent[8][3] = 20;
        _unitSpawnPercent[8][4] = 3;

        // 플레이어 10레벨
        _unitSpawnPercent[9][0] = 19;
        _unitSpawnPercent[9][1] = 25;
        _unitSpawnPercent[9][2] = 25;
        _unitSpawnPercent[9][3] = 25;
        _unitSpawnPercent[9][4] = 6;

        _unitGoldPoolDic = new Dictionary<int, List<int>>();
        for (int i = 0; i < _unitSpawnPercent.Length; i++)
        {
            List<int> temp = new List<int>();
            int[] percent = _unitSpawnPercent[i];
            for (int j = 0; j < percent.Length; j++)
                for (int k = 0; k < percent[j]; k++)
                    temp.Add(j);
            _unitGoldPoolDic.Add(i, temp);
        }

        Unit[] units = Resources.LoadAll<Unit>("Unit/");
        _unitIDDic = new Dictionary<int, List<int>>();
        for (int i = 0; i < Constants.unitMaxGold; i++)
            _unitIDDic.Add(i, new List<int>());
        for (int i = 0; i < units.Length; i++)
            _unitIDDic[units[i].gold-1].Add(units[i].unitID);
    }
}
