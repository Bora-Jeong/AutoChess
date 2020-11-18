using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private GameObject _devCheat;

    private MonoPanel[] _panels;
    private void Awake()
    {
        _panels = GetComponentsInChildren<MonoPanel>(true);
        for(int i = 0; i < _panels.Length; i++)
        {
            _panels[i].Init();
            _panels[i].Hide();
        }
        GameManager.instance.SetCamera(Cam.Lobby);
        LoginPanel.instance.Show();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(2))
            _devCheat.SetActive(!_devCheat.activeSelf);
    }

    public void PlayButtonClickSfx() 
    {
        SoundManager.PlaySFX("button_click");
    }

    public void PlayButtonHoverSfx()
    {
        SoundManager.PlaySFX("button_hover");
    }
}
