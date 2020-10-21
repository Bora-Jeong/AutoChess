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
    private Unit _unit;

    public void Clear ()
    {
        if(_unit != null)
            ObjectPoolManager.instance.ReleaseUnit(_unit);
        _unit = null;
    }

    public void SpawnUnit(int unitID)
    {
        _unit = ObjectPoolManager.instance.GetUnit(unitID);
        _unit.transform.SetParent(transform);
        _unit.transform.localPosition = Vector3.zero;
        _unit.transform.localEulerAngles = Vector3.zero;
    }

}
