using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private Camera[] _cameras;

    public bool isPlaying { get; private set; }

    private GameState _gameState;
    public GameState gameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int round { get; private set; }

    private RoundResult _result; 
    private int _succWin = 0; // 연승 횟수
    private int _succLose = 0; // 연패 횟수

    private int _totalWin; // 승리한 라운드 횟수
    private int _totalLose; // 패배한 라운드 횟수

    public event EventHandler OnGameStateChanged;

    private float _timer;

    private void Awake()
    {
        OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        switch (gameState)
        {
            case GameState.Prepare:
                if (round == Constants.maxRound) // 플레이어 승리
                {
                    StopAllCoroutines();
                    PacketManager.instance.SendPacket_ReportResult(true);
                    GamePanel.instance.GameOver(true, _totalWin, _totalLose);
                    return;
                }
                _timer = Constants.prepareTimeSpan;
                round++;
                if(round > 1)
                {
                    Player.instance.exp++;
                    RewardGold();
                }
                ShopPanel.instance.Show();
                ShopPanel.instance.Shuffle();
                break;

            case GameState.Wait:
                _timer = Constants.waitTimeSpan;
                break;

            case GameState.Battle:
                _timer = Constants.battleTimeSpan;
                break;

            case GameState.Result:
                _timer = Constants.resultTimeSpan;
                _result = FieldManager.instance.GetRoundResult(out int damage);
                if (_result == RoundResult.Lose && damage > 0)
                {
                    Player.instance.hp -= damage * 3.5f;
                    if (Player.instance.hp <= 0) // 플레이어 패배
                    {
                        StopAllCoroutines();
                        ApplyRoundResult();
                        PacketManager.instance.SendPacket_ReportResult(false);
                        GamePanel.instance.GameOver(false, _totalWin, _totalLose);
                        return;
                    }
                }
                ApplyRoundResult();
                break;
        }
    }

    public void StartGame()
    {
        InventoryManager.instance.Clear();
        FieldManager.instance.Clear();
        Player.instance.hp = 100;
        Player.instance.level = 1;
        Player.instance.exp = 0;
        Player.instance.gold = 50;
        round = 0;
        _succWin = _succLose = 0;
        _totalWin = _totalLose = 0;
        SetCamera(Cam.Game);
        LobbyPanel.instance.Hide();
        GamePanel.instance.Show();
        StartCoroutine(GameScheduler());
    }

    public void ExitGame() // 중간에 게임 종료
    {
        StopAllCoroutines();
        SetCamera(Cam.Lobby);
        isPlaying = false;
        ShopPanel.instance.Hide();
        GamePanel.instance.Hide();
        LobbyPanel.instance.Show();
    }

    private void ApplyRoundResult()
    {
        if (_result == RoundResult.Win) // 연승 기록 
        {
            _totalWin++;
            _succWin++;
            _succLose = 0;
        }
        else if (_result == RoundResult.Lose) // 연패 기록
        {
            _totalLose++;
            _succWin = 0;
            _succLose++;
        }
    }

    private void RewardGold()
    {
        int roundGold = round < 5 ? round : 5; // 기본으로 주는 골드
        int winGold = _result == RoundResult.Win ? 1 : 0; // 승리시 +1
        int sucWinGold = 0; // 연승 보너스
        if (_succWin >= 9) sucWinGold = 3;
        else if (_succWin >= 6) sucWinGold = 2;
        else if (_succWin >= 3) sucWinGold = 1;
        int sucLoseGold = 0; // 연패 보너스
        if (_succLose >= 9) sucLoseGold = 3;
        else if (_succLose >= 6) sucLoseGold = 2;
        else if (_succLose >= 3) sucLoseGold = 1;
        int interest = Mathf.Min(Player.instance.gold / 10, 5); // 이자 10퍼센트 최대 5원
        Player.instance.gold += roundGold + winGold + sucWinGold + sucLoseGold + interest;
    }

    private IEnumerator GameScheduler()
    {
        gameState = GameState.Prepare;  // 게임 시작
        _timer = Constants.prepareTimeSpan;
        isPlaying = true;
        
        while (_timer > 0 && isPlaying)
        {
            _timer = Math.Max(_timer - Time.deltaTime, 0);
            string minutes = Mathf.Floor(_timer / 60).ToString("00");
            string seconds = (_timer % 60).ToString("00");
            _timerText.text = string.Format("{0}:{1}", minutes, seconds);
            yield return null;

            if (_timer <= 0)
            {
                gameState = (GameState)(((int)gameState + 1) % Enum.GetValues(typeof(GameState)).Length);
            }
        }
    }

    public void SetCamera(Cam cam)
    {
        for (int i = 0; i < _cameras.Length; i++)
            _cameras[i].enabled = i == (int)cam;

        AudioClip audioClip = SoundManager.GetCurrentSong();
        if (audioClip == null) return;
        if ((cam == Cam.Lobby && audioClip.name == "bgm_game") || (cam == Cam.Game && audioClip.name == "bgm_lobby"))
            SoundManager.Next();
    }
}