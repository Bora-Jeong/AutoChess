using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCanvas : MonoBehaviour
{
    [SerializeField]
    private Image _hp;
    [SerializeField]
    private Image _mp;

    private Unit _owner;
    private void Awake()
    {
        _owner = GetComponentInParent<Unit>();
        GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;
    }

    private void OnEnable()
    {
        Instance_OnGameStateChanged(this, null);
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    { 
        gameObject.SetActive((GameManager.instance.gameState == GameState.Battle || GameManager.instance.gameState == GameState.Result) 
            &&  _owner.onCell != null && _owner.onCell.type == Cell.Type.MyField);
    }

    private void Update()
    {
        float x = Camera.main.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(x, 0, 0);
        _hp.fillAmount = _owner.curHp / _owner.fullHpOnBattle;
        _mp.fillAmount = _owner.curMp / _owner.curFullMp;
        transform.localScale /= transform.lossyScale.x;
    }
}
