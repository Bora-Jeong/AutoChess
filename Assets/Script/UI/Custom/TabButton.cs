using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGruop;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeseleted;

    public Graphic[] graphics;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGruop.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGruop.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGruop.OnTabExit(this);
    }

    private void Start()
    {
        
    }

    public void Select()
    {
        onTabSelected?.Invoke();
    }

    public void DeSelect()
    {
        onTabDeseleted?.Invoke();
    }
}
