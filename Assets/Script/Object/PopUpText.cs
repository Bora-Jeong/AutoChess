using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _textMeshPro;

    private readonly float _moveSpeed = 5f;
    private readonly float _disappearSpeed = 2f;
    private readonly float _maxDiappearTime = 0.5f;
    private readonly float _increaseScaleAmount = 1f;
    private readonly float _decreaseScaleAmount = 1f;

    private float _disappearTimer;
    private Color _textColor;
    private Vector3 _moveVector;

    private static int sortingOrder;

    public static PopUpText Create(Unit unit, string content, bool isCritical = false)
    {
        PopUpText popUpText = ObjectPoolManager.instance.GetObject(ObjectPoolManager.PoolObj.PopUpText).GetComponent<PopUpText>();
        popUpText.SetUp(content, isCritical);
        Vector3 pos = unit.transform.position;
        pos.y += 1.3f;
        popUpText.transform.position = pos;
        return popUpText;
    }

    private void SetUp(string content, bool isCritical)
    {
        _textMeshPro.text = content;
        _disappearTimer = _maxDiappearTime;
        transform.localScale = Vector3.one;

        if (isCritical)
        {
            _textMeshPro.fontSize = 7f;
            _textColor = "FF2B00".HexToColor();
        }
        else
        {
            _textMeshPro.fontSize = 5f;
            _textColor = "FFC500".HexToColor();
        }

        _textColor.a = 1f;
        _textMeshPro.color = _textColor;

        sortingOrder++;
        _textMeshPro.sortingOrder = sortingOrder;

        // 랜덤으로 방향 지정
        float x = Random.Range(-1.3f, 1.3f);
        _moveVector = new Vector3(x, 1) * _moveSpeed;
    }

    private void Update()
    {
        float x = Camera.main.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(x, 0, 0);

        transform.position += _moveVector * Time.deltaTime;
        _moveVector -= _moveVector * 8f * Time.deltaTime;

        if (_disappearTimer > _maxDiappearTime * 0.5f)
        {
            transform.localScale += Vector3.one * _increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            transform.localScale -= Vector3.one * _decreaseScaleAmount * Time.deltaTime;
        }

        _disappearTimer -= Time.deltaTime;
        if(_disappearTimer < 0)
        {
            _textColor.a -= _disappearSpeed * Time.deltaTime;
            _textMeshPro.color = _textColor;
            if(_textColor.a <= 0)
            {
                ObjectPoolManager.instance.Release(gameObject, ObjectPoolManager.PoolObj.PopUpText);
            }
        }

    }
}
