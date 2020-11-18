using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPanel : PanelBase<PopUpPanel>
{
    [SerializeField]
    private GameObject _border;
    [SerializeField]
    private Text _contentText;
    [SerializeField]
    private Button _leftButton;
    [SerializeField]
    private Text _leftButtonText;
    [SerializeField]
    private Button _rightButton;
    [SerializeField]
    private Text _rightButtonText;

    public override void Show()
    {
        base.Show();
        _border.transform.localScale = Vector3.zero;
        LeanTween.scale(_border, Vector3.one, 0.3f).setEaseInBack();
    }

    public void PopUpNotice(string content)
    {
        _contentText.text = content;
        _rightButton.gameObject.SetActive(false);
        _leftButtonText.text = "확인";
        _leftButton.onClick.RemoveAllListeners();
        _leftButton.onClick.AddListener(() =>
        {
            Hide();
        });
        Show();
    }

    public void PopUpYesOrNo(string content, System.Action yesEvent, System.Action noEvent)
    {
        _contentText.text = content;
        _rightButton.gameObject.SetActive(true);
        _leftButtonText.text = "아니오";
        _rightButtonText.text = "예";
        _leftButton.onClick.RemoveAllListeners();
        _leftButton.onClick.AddListener(() =>
        {
            noEvent?.Invoke();
            Hide();
        });
        _rightButton.onClick.RemoveAllListeners();
        _rightButton.onClick.AddListener(() =>
        {
            yesEvent?.Invoke();
            Hide();
        });
        Show();
    }

    public void PopUpReconnect(System.Action reconnectEvent)
    {
        _contentText.text = "서버 연결에 실패했습니다";
        _rightButton.gameObject.SetActive(true);
        _leftButtonText.text = "닫기";
        _rightButtonText.text = "재접속";
        _leftButton.onClick.RemoveAllListeners();
        _leftButton.onClick.AddListener(() =>
        {
            Hide();
        });
        _rightButton.onClick.RemoveAllListeners();
        _rightButton.onClick.AddListener(() =>
        {
            reconnectEvent?.Invoke();
            Hide();
        });
        Show();
    }
}
