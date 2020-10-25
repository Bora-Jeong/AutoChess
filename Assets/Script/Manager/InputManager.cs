using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerDownHandler
{
    private Unit _grabedUnit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ShopPanel.instance.isActiveAndEnabled) // Tab : 상점 On/ Off
                ShopPanel.instance.Hide();
            else
                ShopPanel.instance.Show();
        }
        if (Input.GetKeyDown(KeyCode.E) && _grabedUnit != null) // E : 유닛 판매
        {
            if (GameManager.instance.gameState == GameState.Prepare)
                SellGrabedUnit();
            else if (_grabedUnit.onCell.type == Cell.Type.Inventory)
                SellGrabedUnit();
        }
        if (Input.GetKeyDown(KeyCode.F)) // F : 경험치 구매
            ShopPanel.instance.BuyExp();
    }

    private void SellGrabedUnit()
    {
        InventoryManager.instance.SellUnit(_grabedUnit);
        Select(null);
    }

    private void Select(Unit unit)
    {
        if(_grabedUnit != null)
        {
            _grabedUnit.Ouline(false);
            _grabedUnit = null;
        }
        _grabedUnit = unit;
        if(_grabedUnit != null)
        {
            _grabedUnit.Ouline(true);
        }
        GamePanel.instance.ShowUnitInfo(_grabedUnit);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Unit")))
            {
                Unit unit = hit.collider.GetComponentInParent<Unit>();
                if(unit != null && unit == _grabedUnit)
                {
                    Select(null);
                    return;
                }
                if (unit != null && !unit.isCreep)
                {
                    Select(unit);
                    return;
                }
            }
            if (_grabedUnit != null) // 선택한 유닛이 있다면 이동할 타일 검색
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Cell"))) // 셀 클릭
                {
                    Cell cell = hit.collider.GetComponent<Cell>();
                    if (cell != null && cell.type != Cell.Type.EnemyField && !cell.isOccupied) // 클릭한 유닛 이동
                    {
                        if(GameManager.instance.gameState == GameState.Prepare)
                            FieldManager.instance.MoveUnitToCell(_grabedUnit, cell);
                        else if(_grabedUnit.onCell.type == Cell.Type.Inventory && cell.type == Cell.Type.Inventory)
                            FieldManager.instance.MoveUnitToCell(_grabedUnit, cell);
                    }
                }
                Select(null);
            }
        }
    }
}