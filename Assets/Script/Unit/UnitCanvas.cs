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
        if ( GameManager.instance.gameState == GameManager.GameState.Prepare || GameManager.instance.gameState == GameManager.GameState.Wait)
            gameObject.SetActive(false);
        else if (_owner.onCell != null &&  _owner.onCell.type == Cell.Type.MyField)
            gameObject.SetActive(true);
    }

    private void Update()
    {
        float x = Camera.main.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(x, 0, 0);
        _hp.fillAmount = _owner.curHp / _owner.curFullHp;
        _mp.fillAmount = _owner.curMp / _owner.curFullMp;
        transform.localScale /= transform.lossyScale.x;
    }
}
