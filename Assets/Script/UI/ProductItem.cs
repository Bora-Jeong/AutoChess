using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductItem : MonoBehaviour
{
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text _classText;
    [SerializeField]
    private Text _speciesText;
    [SerializeField]
    private Text _goldText;

    private Unit _unit;
    public void SetUp(Unit unit)
    {
        if (_unit != null)
            ObjectPoolManager.instance.ReleaseUnit(_unit);

        _unit = unit;
        _nameText.text = _unit.Data.Name;
        _classText.text = _unit.Data.CLAS.ToName();
        _speciesText.text = _unit.Data.SPECIES.ToName();
        _goldText.text = $"Gold {_unit.Data.Gold}";
    }

    public void OnClick()
    {
        ShopPanel.instance.BuyUnit(_unit);
    }
}
