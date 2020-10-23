using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : PanelBase<LoadingPanel>
{
    [SerializeField]
    private Slider _loadingBar;

    public void LoadAllUnit() // 모든 유닛을 한개씩 미리 로드해놓음
    {
        _loadingBar.value = 0;
        var unitDic = TableData.instance.unitTableDic;
        int count = 0; 
        foreach(var pair in unitDic)
        {
            Unit unit = ObjectPoolManager.instance.GetUnit(pair.Key);
            ObjectPoolManager.instance.ReleaseUnit(unit);
            count++;
            _loadingBar.value = count / (float)unitDic.Count;
        }
        LobbyPanel.instance.Show();
        Hide();
    }
}
