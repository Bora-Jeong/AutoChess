using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _textMeshPro;

    public static StaticText Create(Unit unit, string content)
    {
        StaticText staticText = ObjectPoolManager.instance.GetObject(ObjectPoolManager.PoolObj.StaticText).GetComponent<StaticText>();
        Vector3 pos = unit.transform.position;
        pos.y += 1.3f;
        staticText.transform.position = pos;
        staticText.SetUp(content);
        return staticText;
    }

    private void SetUp(string content)
    {
        _textMeshPro.text = content;
    }

    private void Update()
    {
        float x = Camera.main.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(x, 0, 0);
    }

}
