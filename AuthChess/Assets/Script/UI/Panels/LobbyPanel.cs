using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : PanelBase<LobbyPanel>
{
    public void OnStartButtonClick()
    {
        GameManager.instance.StartGame();
    }   
}
