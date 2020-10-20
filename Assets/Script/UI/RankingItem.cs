using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingItem : MonoBehaviour
{
    [SerializeField]
    private Text _rankText;
    [SerializeField]
    private Text _nicknameText;
    [SerializeField]
    private Text _winText;
    [SerializeField]
    private Text _loseText;
    [SerializeField]
    private Text _rateText;

    public void SetUp(int rank, string nickname, int win, int lose, float rate)
    {
        _rankText.text = $"{rank}등";
        _nicknameText.text = nickname;
        _winText.text = win.ToString();
        _loseText.text = lose.ToString();
        _rateText.text = $"{rate * 100}%";
    }
}
