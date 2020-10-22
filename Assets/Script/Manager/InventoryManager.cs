using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField]
    private Cell[] _cells;

    public void Clear()
    {
        for(int i = 0; i < _cells.Length; i++)
        {
            _cells[i].DespawnUnit();
        }
    }

    public bool IsFull()
    {
        for(int i = 0; i < _cells.Length; i++)
        {
            if (!_cells[i].isOccupied)
                return false;
        }
        return true;
    }

    public void AddUnit(int unitID)
    {
        for(int i = 0; i < _cells.Length; i++)
        {
            if (!_cells[i].isOccupied)
            {
                _cells[i].SpawnUnit(unitID);
                break;
            }
        }
    }
}
