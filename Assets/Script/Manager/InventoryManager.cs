using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField]
    private Cell[] _cells;

    private bool _isMerged = false;
    private Dictionary<int, List<Unit>> _myUnits = new Dictionary<int, List<Unit>>(); // 모든 나의 유닛들 key : unitID

    public void BuyUnit(int unitID) // 상점에서 구매
    {
        for(int i = 0; i < _cells.Length; i++)
        {
            if (!_cells[i].isOccupied)
            {
                Unit unit = _cells[i].SpawnUnit(unitID);
                if (_myUnits.TryGetValue(unit.Data.Unitid, out List<Unit> list))
                    list.Add(unit);
                else
                {
                    List<Unit> newList = new List<Unit>();
                    newList.Add(unit);
                    _myUnits.Add(unit.Data.Unitid, newList);
                }
                break;
            }
        }
        FindUnitsToMerge(); // 자동 조합
    }

    private void FindUnitsToMerge()
    {
        List<Unit> star1 = new List<Unit>(), star2 = new List<Unit>();
        _isMerged = false;

        foreach (var pair in _myUnits) // star1에 1성 유닛, star2에 2성 유닛 추가
        {
            if (pair.Value.Count < 3) continue;
            star1.Clear();
            star2.Clear();

            for (int i = 0; i < pair.Value.Count; i++)
                if (pair.Value[i].star == 1) star1.Add(pair.Value[i]);
            if (star1.Count >= 3) MergeUnits(star1);

            for (int i = 0; i < pair.Value.Count; i++)
                if (pair.Value[i].star == 2) star2.Add(pair.Value[i]);
            if (star2.Count >= 3) MergeUnits(star2);
        }

        if (_isMerged)
            FieldManager.instance.FieldChanged();
    }

    private void MergeUnits(List<Unit> units) // 입력은 unitID와 star가 같은 유닛들의 리스트
    {
        _isMerged = true;
        while (units.Count >= 3)
        {
            units[0].star++;
            for (int i = 1; i < 3; i++)
            {
                DestroyUnit(units[i]);
            }
            units.RemoveRange(0, 3);
        }
    }

    public void SellUnit(Unit unit)
    {
        Player.instance.gold += unit.curGold;
        DestroyUnit(unit);
    }

    public void DestroyUnit(Unit unit) // 합쳐지면서 사라지는 유닛
    {
        if (_myUnits.TryGetValue(unit.Data.Unitid, out List<Unit> list))
            list.Remove(unit);
        FieldManager.instance.DestroyUnit(unit);
        unit.onCell.DeSetUnit();
        FieldManager.instance.FieldChanged();
        ObjectPoolManager.instance.Release(unit);
    }

    public void Clear()
    {
        foreach(var pair in _myUnits)
            for (int i = 0; i < pair.Value.Count; i++)
                DestroyUnit(pair.Value[i]);
    }

    public void ClearMyField()
    {
        foreach (var pair in _myUnits)
            for (int i = 0; i < pair.Value.Count; i++)
                if(pair.Value[i].onCell.type == Cell.Type.MyField)
                    DestroyUnit(pair.Value[i]);
    }

    public bool IsFull()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            if (!_cells[i].isOccupied)
                return false;
        }
        return true;
    }
}
