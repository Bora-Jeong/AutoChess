using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCanvas : MonoBehaviour
{
    [SerializeField]
    private Image _hp;
    [SerializeField]
    private Image _mp;

    private Unit _owner;

    private void Start()
    {
        _owner = GetComponentInParent<Unit>();
        gameObject.SetActive(false);
    }
    private void Update()
    {
        float x = Camera.main.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(x, 0, 0);
        _hp.fillAmount = _owner.curHp / _owner.Data.Hp;
        _mp.fillAmount = _owner.curMp / 100;
    }
}
