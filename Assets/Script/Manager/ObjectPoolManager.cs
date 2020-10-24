using QFSW.MOP2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFX.IFX;
using System.Linq;


public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [SerializeField]
    private GameObject[] _poolObjs;

    [SerializeField]
    private IFX_AbilityFx[] AbilityFxs;

    public enum PoolObj
    {
        PopUpText,
        StaticText
    }

    public enum FxType
    {
        Shield
    }

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
            ObjectPool objectPool = ObjectPool.CreateAndInitialize(unit.gameObject, 5);
            MasterObjectPooler.Instance.AddPool(objectPool);
            return MasterObjectPooler.Instance.GetObject(name).GetComponent<Unit>();
        }
    }

    public GameObject GetObject(PoolObj poolObject)
    {
        ObjectPool pool = MasterObjectPooler.Instance.GetPool(poolObject.ToString());
        if (pool != null)
        {
            return pool.GetObject();
        }
        else
        {
            ObjectPool objectPool = ObjectPool.CreateAndInitialize(_poolObjs[(int)poolObject], poolObject.ToString());
            MasterObjectPooler.Instance.AddPool(objectPool);
            return MasterObjectPooler.Instance.GetObject(poolObject.ToString());
        }
    }
    public void Release(Unit unit)
    {
        MasterObjectPooler.Instance.Release(unit.gameObject, $"unit_{unit.Data.Unitid}");
    }

    public void Release(GameObject gameObject, PoolObj poolObj)
    {
        MasterObjectPooler.Instance.Release(gameObject, poolObj.ToString());
    }

    public void LaunchFx(Transform parent, FxType fxType)
    {
        string abilityName = "";
        switch (fxType)
        {
            case FxType.Shield:
                abilityName = "Dark Paralysis";
                break;
        }

        var abilityFx = AbilityFxs.SingleOrDefault(t => t.FxName == abilityName);

        if (abilityFx == default(IFX_AbilityFx))
            return;

        var abilityGo = Instantiate(abilityFx.Fx, parent);
        abilityGo.transform.localPosition = Vector3.zero;
        abilityGo.transform.position +=  abilityFx.Offset;


        if (abilityFx.LaunchAudioClip != null)
        {
            var abilityAudioSource = abilityGo.GetComponent<AudioSource>();
            if (abilityAudioSource != null)
                abilityAudioSource.PlayOneShot(abilityFx.LaunchAudioClip);
        }

        var ability = abilityGo.GetComponentInChildren<IFX_IAbilityFx>();
        if (ability != null)
            ability.Launch();
    }
}
