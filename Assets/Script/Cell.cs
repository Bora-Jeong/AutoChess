using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Type
    {
        Inventory,
        MyField,
        EnemyField
    }

    [SerializeField]
    private Type _type;

    public Type type => _type;
    public bool isOccupied => _unit != null;

    public Unit _unit; // debulg용 public

    public void SetUnit(Unit unit)
    {
        _unit = unit;
        _unit.onCell.DeSetUnit(); // 이전에 있던 Cell에 알림
        _unit.onCell = this;
    }

    public void DeSetUnit()
    {
        _unit.onCell = null;
        _unit = null;
    }

    public Unit SpawnUnit(int unitID)
    {
        _unit = ObjectPoolManager.instance.GetUnit(unitID);
        _unit.transform.position = transform.position;
        _unit.transform.localEulerAngles = Vector3.zero;
        _unit.isPlayerUnit = true;
        _unit.onCell = this;
        return _unit;
    }

    public void DespawnUnit()
    {
        if (_unit != null)
        {
            ObjectPoolManager.instance.ReleaseUnit(_unit);
            _unit.onCell = null;
        }
        _unit = null;
    }
}
