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

    public event EventHandler OnFieldChanged;

    public void MoveUnitToCell(Unit unit, Cell dest)
    {
        bool fieldChanged = unit.onCell.type != dest.type; // 인벤토리 <-> 필드로 이동

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
