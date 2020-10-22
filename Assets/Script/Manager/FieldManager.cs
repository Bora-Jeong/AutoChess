using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;

public class FieldManager : Singleton<FieldManager>
{
    [SerializeField]
    private Cell[] _myCells;
    [SerializeField]
    private Cell[] _enemyCells;

    private Dictionary<int, List<Unit>> _unitsOnMyField = new Dictionary<int, List<Unit>>(); // 내 필드에 있는 유닛 key : uniID
    private Coroutine _fieldChangedCor;

    public event EventHandler OnFieldChanged;

    public void MoveUnitToCell(Unit unit, Cell dest)
    {
        bool fieldChanged = unit.onCell.type != dest.type;
        if (fieldChanged) // 인벤 <-> 필드간 이동이면
        {
            if(unit.onCell.type == Cell.Type.Inventory) // 인벤토리 -> 필드
            {
                if(_unitsOnMyField.TryGetValue(unit.unitID, out List<Unit> list))
                    list.Add(unit);
                else
                {
                    List<Unit> newList = new List<Unit>();
                    newList.Add(unit);
                    _unitsOnMyField.Add(unit.unitID, newList);
                }
            }
            else // 필드 -> 인벤토리
                _unitsOnMyField[unit.unitID].Remove(unit);
        }

        dest.SetUnit(unit);
        unit.MoveToCell(dest);

        if (fieldChanged)
        {
            if (_fieldChangedCor != null) StopCoroutine(_fieldChangedCor);
            _fieldChangedCor = StartCoroutine(FieldChanged());
        }
    }

    private IEnumerator FieldChanged()
    {
        yield return new WaitForSeconds(0.8f);

        FindUnitsToMerge();
        UpdateSynergy();
        OnFieldChanged?.Invoke(this, EventArgs.Empty);

        _fieldChangedCor = null;
    }

    private void FindUnitsToMerge()
    {
        List<Unit> star1 = new List<Unit>(), star2 = new List<Unit>();
        foreach (var pair in _unitsOnMyField) // star1에 1성 유닛, star2에 2성 유닛 추가
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
    }

    private void MergeUnits(List<Unit> units) // 입력은 unitID와 star가 같은 유닛들의 리스트
    {
        while(units.Count >= 3)
        {
            units[0].star++;
            for(int i = 1; i <3; i++)
            {
                _unitsOnMyField[units[i].unitID].Remove(units[i]);
                units[i].onCell.DeSetUnit();
                ObjectPoolManager.instance.ReleaseUnit(units[i]);
            }
            units.RemoveRange(0, 3);
        }
    }

    private void UpdateSynergy() // 시너지 업데이트
    {

    }

    public void Clear()
    {
        for(int i = 0; i < _myCells.Length; i++)
        {
            _myCells[i].DespawnUnit();
            _enemyCells[i].DespawnUnit();
        }
        _unitsOnMyField.Clear();
        OnFieldChanged?.Invoke(this, EventArgs.Empty);
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

}
