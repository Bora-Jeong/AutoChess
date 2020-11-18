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
            ObjectPoolManager.instance.Release(_unit);

        _unit = unit;
        _nameText.text = _unit.Data.Name;
        _nameText.color = _unit.Data.Gold.GoldColor();
        _classText.text = _unit.Data.CLAS.ToName();
        _speciesText.text = _unit.Data.SPECIES.ToName();
        _goldText.text = _unit.Data.Gold.ToString();
        _goldText.color = unit.Data.Gold.GoldColor();
        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        if (ShopPanel.instance.BuyUnit(_unit))
            gameObject.SetActive(false);
    }
}
