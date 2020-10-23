using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using System.Threading;

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

    private Dictionary<int, int> _unitsOnField = new Dictionary<int, int>(); // key : unitID , value : count

    public Dictionary<int, int> unitsOnField => _unitsOnField;

    public void MoveUnitToCell(Unit unit, Cell dest)
    {
        bool fieldChanged = unit.onCell.type != dest.type; // 인벤토리 <-> 필드로 이동

        if (fieldChanged)
        {
            if(dest.type == Cell.Type.MyField)  // 인벤토리 -> 필드
            {
                if (_unitsOnField.ContainsKey(unit.Data.Unitid))
                    _unitsOnField[unit.Data.Unitid]++;
                else
                    _unitsOnField.Add(unit.Data.Unitid, 1);
            }
            else // 필드 -> 인벤토리
            {
                _unitsOnField[unit.Data.Unitid]--;
            }
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
        
    }

    public void DestroyUnit(Unit unit) // 근본적인 삭제는 Inventory Manager에서
    {
        if (unit.onCell.type == Cell.Type.MyField)
            _unitsOnField[unit.unitID]--;
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

    public void Clear()
    {
        _unitsOnField.Clear();
    }

    public Sprite GetClassIcon(Clas clas) => _classIcon[(int)clas];
    public Sprite GetSpeciesIcon(Species species) => _speciesIcon[(int)species];

}
