using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    private RectTransform _rectTransform;
    private float _width;
    private bool _isOn;

    private readonly float moveTime = 0.4f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _width = _rectTransform.rect.width;
    }

    public void OnItemShopButton()
    {
        LeanTween.value(gameObject, _rectTransform.anchoredPosition.x, _isOn ? _width : 0, moveTime).setOnUpdate((float x) =>
        {
            _rectTransform.anchoredPosition = new Vector2(x, _rectTransform.anchoredPosition.y);
        });
        _isOn = !_isOn;
    }
}
