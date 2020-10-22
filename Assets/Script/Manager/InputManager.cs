using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector3 _prevInputPos;
    private bool _isDragged;

    private Unit _grabedUnit;

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            _isDragged = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _prevInputPos = Input.mousePosition;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Unit")))
            {
                Unit unit = hit.collider.GetComponentInParent<Unit>();
                if (unit != null && unit.isPlayerUnit)
                {
                    if (_grabedUnit != null) _grabedUnit.Ouline(false);
                    _grabedUnit = unit;
                    _grabedUnit.Ouline(true);
                    return;
                }
            }
            if (_grabedUnit != null) // 선택한 유닛이 있다면 이동할 타일 검색
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Cell")))
                {
                    Cell cell = hit.collider.GetComponent<Cell>();
                    if (cell != null && cell.type != Cell.Type.EnemyField && !cell.isOccupied) // 클릭한 유닛 이동
                    {
                        FieldManager.instance.MoveUnitToCell(_grabedUnit, cell);
                    }
                }
                if (_grabedUnit != null) _grabedUnit.Ouline(false);
                _grabedUnit = null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
        }
    }

}