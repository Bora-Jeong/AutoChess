using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FieldManager : Singleton<FieldManager>
{
    [SerializeField]
    private Cell[] _myCells;
    [SerializeField]
    private Cell[] _enemyCells;
    [SerializeField]
    private Sprite[] _classIcon;
    [SerializeField]
    private Sprite[] _speciesIcon;

    public event EventHandler OnFieldChanged;

    private Dictionary<int, List<Unit>> _myUnitsOnField = new Dictionary<int, List<Unit>>(); // key : unitID
    private List<Unit> _enemyUnitsOnField = new List<Unit>();
    private Cell[] _fieldCells = new Cell[(int)Math.Pow(Constants.fieldRow, 2)];

    public Dictionary<Clas, int> curClassSynergy = new Dictionary<Clas, int>();
    public Dictionary<Species, int> curSpeciesSynergy = new Dictionary<Species, int>();

    private void Awake()
    {
        GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;
        for (int i = 0; i < _myCells.Length; i++)
            _fieldCells[i] = _myCells[i];
        for (int i = 0; i < _enemyCells.Length; i++)
            _fieldCells[_myCells.Length + i] = _enemyCells[i];
    }

    private void Instance_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.instance.gameState == GameState.Wait)
        {
            SetUnitCountUnderLimit();
            SpawnEnemys();
            ApplyAllSynergyEffect();
        }
        else if (GameManager.instance.gameState == GameState.Prepare)
        {
            ClearEnemyField();
        }
    }

    public void ReportRoundFinish()
    {
        if(GetRoundResult(out int damage) != RoundResult.Draw) // 시간이 끝나기 전에 한쪽이 전멸하면 라운드 종료
        {
            GameManager.instance.gameState++;
        }
    }

    private void SetUnitCountUnderLimit()
    {
        int count = 0;
        foreach(var pair in _myUnitsOnField)
            count += pair.Value.Count;

        count -= Player.instance.level;
        if (count <= 0) return;

        foreach (var pair in _myUnitsOnField) // 초과한 유닛수만큼 판매
        {
            if (pair.Value.Count <= 0) continue;
            for(int i = 0; i < pair.Value.Count; i++)
            {
                InventoryManager.instance.SellUnit(pair.Value[i]);
                count--;
                if (count <= 0) return;
            }
        }
    }

    private void SpawnEnemys()
    {
        List<Unit> playerUnit = new List<Unit>();
        foreach (var pair in _myUnitsOnField)
            for (int i = 0; i < pair.Value.Count; i++)
                playerUnit.Add(pair.Value[i]);

        Dictionary<int, List<int>> unitIDDic = ShopPanel.instance.unitIDDic;

        while(_enemyUnitsOnField.Count < Player.instance.level)
        {
            Unit unit = null;
            if (playerUnit.Count > 0)
            {
                Unit temp = playerUnit[UnityEngine.Random.Range(0, playerUnit.Count)]; // 플레이어 유닛 중 하나 랜덤으로 뽑음
                playerUnit.Remove(temp);
                int randomInt = UnityEngine.Random.Range(0, 10); // 랜덤으로 플레이어 골드 +- 1
                if (randomInt < 2) randomInt = -1;
                else if (randomInt < 4) randomInt = 1;
                else randomInt = 0;
                int unitGold = Mathf.Clamp(temp.Data.Gold + randomInt, 1, Constants.unitMaxGold) - 1; // 유닛 골드
                int unitID = unitIDDic[unitGold][UnityEngine.Random.Range(0, unitIDDic[unitGold].Count)];
                unit = ObjectPoolManager.instance.GetUnit(unitID);
                unit.star = temp.star;
            }
            else
            {
                int unitID = ShopPanel.instance.GetRandomUnitID();
                unit = ObjectPoolManager.instance.GetUnit(unitID);
                int randomInt = UnityEngine.Random.Range(0, 100); // 1성 50퍼 2성 45퍼 3성 5퍼
                if (randomInt < 5) randomInt = 3;  
                else if (randomInt < 50) randomInt = 2;
                else randomInt = 1;
                unit.star = randomInt;
            }
            GetEmptyEnemyCell().SetUnit(unit);
            unit.transform.position = unit.onCell.transform.position;
            unit.transform.localEulerAngles = new Vector3(0, 180, 0);
            unit.isCreep = true;
            _enemyUnitsOnField.Add(unit);
         }
    }

    public void ClearEnemyField()
    {
        while(_enemyUnitsOnField.Count > 0)
        {
            Unit unit = _enemyUnitsOnField[0];
            unit.onCell.DeSetUnit();
            ObjectPoolManager.instance.Release(unit);
            _enemyUnitsOnField.RemoveAt(0);
        }
    }

    public void MoveUnitToCell(Unit unit, Cell dest)
    {
        bool fieldChanged = unit.onCell.type != dest.type; // 인벤토리 <-> 필드로 이동

        if (fieldChanged)
        {
            if (dest.type == Cell.Type.MyField)  // 인벤토리 -> 필드
            {
                if (_myUnitsOnField.ContainsKey(unit.Data.Unitid))
                    _myUnitsOnField[unit.Data.Unitid].Add(unit);
                else
                {
                    List<Unit> newList = new List<Unit>();
                    newList.Add(unit);
                    _myUnitsOnField.Add(unit.Data.Unitid, newList);
                }
            }
            else // 필드 -> 인벤토리
                _myUnitsOnField[unit.Data.Unitid].Remove(unit);
        }

        dest.SetUnit(unit);
        unit.MoveToCell(dest);

        if (fieldChanged)
            FieldChanged();
    }

    public void FieldChanged()
    {
        UpdateSynergy();
        OnFieldChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateSynergy() // 시너지 업데이트
    {
        curClassSynergy.Clear();
        curSpeciesSynergy.Clear();
        foreach (var pair in _myUnitsOnField)
        {
            if (pair.Value.Count <= 0) continue;
            var data = TableData.instance.GetUnitTableData(pair.Key);
            if (curClassSynergy.ContainsKey(data.CLAS)) curClassSynergy[data.CLAS]++;
            else curClassSynergy.Add(data.CLAS, 1);
            if (curSpeciesSynergy.ContainsKey(data.SPECIES)) curSpeciesSynergy[data.SPECIES]++;
            else curSpeciesSynergy.Add(data.SPECIES, 1);
        }
    }

    private void ApplyAllSynergyEffect()
    {
        foreach (var pair in curClassSynergy)
            ApplySynergyEffect(pair.Key, pair.Value);
        foreach (var pair in curSpeciesSynergy)
            ApplySynergyEffect(pair.Key, pair.Value);
    }

    public RoundResult GetRoundResult(out int damage)
    {
        int myUnit = 0;
        foreach(var pair in _myUnitsOnField)
        {
            for (int i = 0; i < pair.Value.Count; i++)
                if (!pair.Value[i].isDead) myUnit++;
        }

        int creep = 0;
        for (int i = 0; i < _enemyUnitsOnField.Count; i++)
            if (!_enemyUnitsOnField[i].isDead) creep++;

        damage = creep;
        if (myUnit > 0 && creep > 0) return RoundResult.Draw;
        else if (creep == 0) return RoundResult.Win;
        else return RoundResult.Lose;
    }

    private void ApplySynergyEffect(Clas clas, int typeCount) // Battle 시작하면 적용
    {
        ClassSynergyTableData data = TableData.instance.GetClassSynergyTableData(clas);

        switch (clas)
        {
            case Clas.Devil: // 모든 아군 유닛의 공격력이 {0}% 증가합니다.
                foreach (var pair in _myUnitsOnField)
                {
                    for(int i = 0; i < pair.Value.Count; i++)
                    {
                        for(int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyAttackPower += pair.Value[i].curAttackPower * data.Values[j] / 100;
                        }
                    }
                }
                break;

            case Clas.Ork: // 모든 아군 유닛의 체력이 {0} 증가합니다.
                foreach (var pair in _myUnitsOnField)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        for (int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyHp += data.Values[j];
                        }
                    }
                }
                break;

            case Clas.Dragon: // 라운드 시작시 아군 용 유닛의 마나가 가득 찹니다.
                    Unit.dragonSynergy = typeCount >= data.Typecount[0];
                break;

            case Clas.Human: // 아군 인간 유닛이 공격시 {0}% 확률로 적을 침묵 시킵니다.
                foreach (var pair in _myUnitsOnField)
                {
                    if (TableData.instance.GetUnitTableData(pair.Key).CLAS != Clas.Human) continue;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        for (int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyMakeSilent += data.Values[j];
                        }
                    }
                }
                break;

            case Clas.Elf: // 아군 엘프 유닛의 회피가 {0} 증가합니다.
                foreach (var pair in _myUnitsOnField)
                {
                    if (TableData.instance.GetUnitTableData(pair.Key).CLAS != Clas.Elf) continue;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        for (int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyEvasion += data.Values[j];
                        }
                    }
                }
                break;
        }
    }

    private void ApplySynergyEffect(Species species, int typeCount)
    {
        SpeciesSynergyTableData data = TableData.instance.GetSpeciesSynergyTableData(species);

        switch (species)
        {
            case Species.Knight: // 3초마다 {0}% 확률로 보호막을 생성합니다.
                Unit.knightSynergy = 0;
                for (int i = 0; i < data.Typecount.Length; i++)
                {
                    if (typeCount < data.Typecount[i]) return;
                    Unit.knightSynergy += data.Values[i];
                }
                break;

            case Species.Mage: // 모든 적군 유닛의 마법 저항력을 {0}% 감소합니다.
                for(int i = 0; i < _enemyUnitsOnField.Count; i++)
                {
                    for(int j = 0; j < data.Typecount.Length; j++)
                    {
                        if (typeCount < data.Typecount[j]) break;
                        _enemyUnitsOnField[i].synergyMagicResistPower -= data.Values[j] / 100;
                    }
                }
                break;

            case Species.Hunter: // 아군 사냥꾼 유닛의 공격력이 {0}% 증가합니다.
                foreach (var pair in _myUnitsOnField)
                {
                    if (TableData.instance.GetUnitTableData(pair.Key).SPECIES != Species.Hunter) continue;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        for (int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyAttackPower += pair.Value[i].curAttackPower * data.Values[j] / 100;
                        }
                    }
                }
                break;

            case Species.Assassin: // 공격시 15% 확률로 {0}배의 피해를 줍니다.
                Unit.assassinSynergyCritical = Unit.assassinSynergyPercent = 0;
                for (int i = 0; i < data.Typecount.Length; i++)
                {
                    if (typeCount < data.Typecount[i]) return;
                    Unit.assassinSynergyPercent += Constants.assassinCriticalPercent;
                    Unit.assassinSynergyCritical += data.Values[i];
                }
                break;

            case Species.Warrior: // 아군 전사 유닛의 방어력이 {0} 증가합니다.
                foreach (var pair in _myUnitsOnField)
                {
                    if (TableData.instance.GetUnitTableData(pair.Key).SPECIES != Species.Warrior) continue;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        for (int j = 0; j < data.Typecount.Length; j++)
                        {
                            if (typeCount < data.Typecount[j]) break;
                            pair.Value[i].synergyDeffensePower += data.Values[j];
                        }
                    }
                }
                break;
        }
    }

    public void DestroyUnit(Unit unit) // 근본적인 삭제는 Inventory Manager에서
    {
        if (unit.onCell.type == Cell.Type.MyField)
        {
            _myUnitsOnField[unit.Data.Unitid].Remove(unit);
        }
    }

    public int GetCountOfChessOnMyField()
    {
        int count = 0;
        for(int i = 0; i < _myCells.Length; i++)
        {
            if (_myCells[i].isOccupied)
                count++;
        }
        return count;
    }

    public Cell GetEmptyEnemyCell() // for test
    {
        for (int i = 0; i < _enemyCells.Length; i++)
            if (!_enemyCells[i].isOccupied)
                return _enemyCells[i];
        return null;
    }

    public void Clear()
    {
        _myUnitsOnField.Clear();
        curClassSynergy.Clear();
        curSpeciesSynergy.Clear();
    }

    public Sprite GetClassIcon(Clas clas) => _classIcon[(int)clas];
    public Sprite GetSpeciesIcon(Species species) => _speciesIcon[(int)species];

}