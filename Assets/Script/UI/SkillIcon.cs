using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private GameObject _skillDescGroup;
    [SerializeField]
    private Text _skillDescText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _skillDescGroup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _skillDescGroup.SetActive(false);
    }

    public void SetUp(Unit unit)
    {
        _skillIcon.sprite = unit.skillIcon;
        _skillDescText.text = $"skillDesc_{unit.Data.Unitid}".Convert();
        _skillDescGroup.SetActive(false);
    }
}
