using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : PanelBase<LoginPanel>
{
    [SerializeField]
    private InputField _nickname;
    [SerializeField]
    private InputField _password;

    private void Start()
    {
        _nickname.text = PlayerPrefs.GetString("Nickname", string.Empty);
        _password.text = PlayerPrefs.GetString("Password", string.Empty);
    }

    public void OnLoginButtonClick()
    {
        UIManager.instance.PlayButtonClickSfx();
        PacketManager.instance.SendPacket_Login(_nickname.text, _password.text);
    }

    public void OnAuthButtonClick()
    {
        UIManager.instance.PlayButtonClickSfx();
        PacketManager.instance.SendPacket_Authentication(_nickname.text, _password.text);
    }

    public void SaveCurrentInfo()
    {
        PlayerPrefs.SetString("Nickname", _nickname.text);
        PlayerPrefs.SetString("Password", _password.text);
        PlayerPrefs.Save();
    }
}
