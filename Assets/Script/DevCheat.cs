using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevCheat : Singleton<DevCheat>
{
    [SerializeField]
    private InputField _unitInputField;

    public bool UNDEAD;


    public void OnSpawnUnit()
    {
        Unit unit = ObjectPoolManager.instance.GetUnit(int.Parse(_unitInputField.text));
        ShopPanel.instance.BuyUnit(unit);
        ObjectPoolManager.instance.Release(unit);
    }

    public void OnSpawnEnemy()
    {
        Unit unit = ObjectPoolManager.instance.GetUnit(int.Parse(_unitInputField.text));
        unit.onCell = FieldManager.instance.GetEmptyEnemyCell();
        unit.onCell._unit = unit;
        unit.isCreep = true;
        unit.transform.position = unit.onCell.transform.position;
        unit.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void OnChangeGameState()
    {
        GameManager.instance.gameState++;
    }

    public void OnClearMyField()
    {
        InventoryManager.instance.ClearMyField();
    }

    public void OnClearEnemyField()
    {
        FieldManager.instance.ClearEnemyField();
    }
}
