using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class HoverDesc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _descGroup;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descGroup.SetActive(true);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        _descGroup.SetActive(false);
    }
}
