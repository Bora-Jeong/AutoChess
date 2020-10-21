using QFSW.MOP2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public Unit GetUnit(int unitID)
    {
        string name = $"unit_{unitID}";
        ObjectPool pool = MasterObjectPooler.Instance.GetPool(name);
        if(pool != null)
        {
            return pool.GetObject().GetComponent<Unit>();
        }
        else
        {
            Unit unit = Resources.Load<Unit>($"Unit/unit_{unitID}");
            ObjectPool objectPool = ObjectPool.CreateAndInitialize(unit.gameObject);
            MasterObjectPooler.Instance.AddPool(objectPool);
            return MasterObjectPooler.Instance.GetObject(name).GetComponent<Unit>();
        }
    }

    public void ReleaseUnit(Unit unit)
    {
        MasterObjectPooler.Instance.Release(unit.gameObject, $"unit_{unit.unitID}");
    }
}
