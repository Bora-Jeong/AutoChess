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
            // 위에 있는 캐릭터 삭제
            _cells[i].isOccupied = false;
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
}
