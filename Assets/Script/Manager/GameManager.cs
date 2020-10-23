using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Text _timerText;

    public enum GameState
    {
        Prepare, 
        Wait, 
        Battle,
        Result 
    }

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
                _timer = Constants.prepareTimeSpan;
                round++;
                ShopPanel.instance.Show();
                ShopPanel.instance.Shuffle();
                break;

            case GameState.Wait:
                _timer = Constants.waitTimeSpan;
                // 몹 소환
                break;

            case GameState.Battle:
                _timer = Constants.battleTimeSpan;
                break;

            case GameState.Result:
                _timer = Constants.resultTimeSpan;
                // 패배시 플레이어 체력 감소, 연승 or 연패 기록
                break;
        }
    }

    public void StartGame()
    {
        InventoryManager.instance.Clear();
        FieldManager.instance.Clear();
        Player.instance.level = 1;
        Player.instance.exp = 0;
        Player.instance.gold = 100; // 개발용
        round = 0;
        LobbyPanel.instance.Hide();
        GamePanel.instance.Show();
        StartCoroutine(GameScheduler());
    }

    private IEnumerator GameScheduler()
    {
        gameState = GameState.Prepare;  // 게임 시작
        _timer = Constants.prepareTimeSpan;
        isPlaying = true;
        
        while (_timer > 0 && isPlaying)
        {
            _timer -= Time.deltaTime;
            string minutes = Mathf.Floor(_timer / 60).ToString("00");
            string seconds = (_timer % 60).ToString("00");
            _timerText.text = string.Format("{0}:{1}", minutes, seconds);
            yield return null;

            if (_timer <= 0)
                gameState = (GameState)(((int)gameState + 1) % Enum.GetValues(typeof(GameState)).Length);
        }
    }
}