using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        HideAllPanels();
        LoginPanel.instance.Show();
    }

    public void HideAllPanels()
    {
        LobbyPanel.instance.Hide();
        GamePanel.instance.Hide();
        ShopPanel.instance.Hide();
        LoginPanel.instance.Hide();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ShopPanel.instance.isActiveAndEnabled)
                ShopPanel.instance.Hide();
            else
                ShopPanel.instance.Show();
        }
    }
}
