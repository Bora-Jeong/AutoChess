using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase<GamePanel>
{
    [SerializeField]
    private Text _roundText;
    [SerializeField]
    private Text _mobNameText;
    [SerializeField]
    private Text _gameStateText;
    [SerializeField]
    private Text _chessCountText;
    [SerializeField]
    private Text _goldText;
    [SerializeField]
    private Text _nicknameText;
    [SerializeField]
    private Text _levelText;
    [SerializeField]
    private Image _expImage;
    [SerializeField]
    private Slider _hpSlider;
    [SerializeField]
    private Text _hpText;


    public override void Init()
    {
        GameManager.instance.OnRoundChanged += Instance_OnRoundChanged;
        GameManager.instance.OnGameStateChanged += Instance_OnGameStateChanged;
        Player.instance.OnLevelChanged += Instance_OnLevelChanged;
        Player.instance.OnExpChanged += Instance_OnExpChanged;
        Player.instance.OnGoldChanged += Instance_OnGoldChanged;
        Player.instance.OnHPChanged += Instance_OnHPChanged;
        FieldManager.instance.OnFieldChanged += Instance_OnFieldChanged;
    }
    private void Start()
    {
        _nicknameText.text = Player.instance.nickname;
    }

    private void Instance_OnFieldChanged(object sender, System.EventArgs e)
    {
        _chessCountText.text = $"{FieldManager.instance.GetCountOfChessOnMyField()}/{Player.instance.level}";
    }

    private void Instance_OnGameStateChanged(object sender, System.EventArgs e)
    {
        _gameStateText.text = GameManager.instance.gameState.ToString().ToUpper();
    }

    private void Instance_OnHPChanged(object sender, System.EventArgs e)
    {
        _hpSlider.value = Player.instance.hp;
        _hpText.text = $"{Player.instance.hp} / 100";
    }

    private void Instance_OnRoundChanged(object sender, System.EventArgs e)
    {
        _roundText.text = $"Round { GameManager.instance.round}";
    }

    private void Instance_OnGoldChanged(object sender, System.EventArgs e)
    {
        _goldText.text = Player.instance.gold.ToString();
    }

    private void Instance_OnExpChanged(object sender, System.EventArgs e)
    {
        _expImage.fillAmount = Player.instance.exp / (float)100;
    }

    private void Instance_OnLevelChanged(object sender, System.EventArgs e)
    {
        _levelText.text = Player.instance.level.ToString();
        _chessCountText.text = $"{FieldManager.instance.GetCountOfChessOnMyField()}/{Player.instance.level}";
    }

}