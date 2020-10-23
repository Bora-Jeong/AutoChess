using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private MonoPanel[] _panels;
    private void Awake()
    {
        _panels = GetComponentsInChildren<MonoPanel>(true);
        for(int i = 0; i < _panels.Length; i++)
        {
            _panels[i].Init();
            _panels[i].Hide();
        }
        LoginPanel.instance.Show();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ShopPanel.instance.isActiveAndEnabled)
                ShopPanel.instance.Hide();
            else
                ShopPanel.instance.Show();
        }
    }
}
