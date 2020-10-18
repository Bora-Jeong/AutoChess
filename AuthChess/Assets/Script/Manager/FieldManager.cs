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

    public event EventHandler OnFieldChanged;

    public void Clear()
    {
        for(int i = 0; i < _myCells.Length; i++)
        {

        }

        OnFieldChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetCountOfMyChessOnField()
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
