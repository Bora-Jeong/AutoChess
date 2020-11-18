using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Singleton<Player>
{
    private int _level;
    public int level
    {
        get => _level;
        set
        {
            _level = value;
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private int _exp;
    public int exp
    {
        get => _exp;
        set
        {
            _exp = value;
            while(_exp >= Constants.requiredExpToLevelUp[level]) // 레벨업
            {
                _exp -= Constants.requiredExpToLevelUp[level];
                level++;
            }
            OnExpChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    private int _gold;
    public int gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnGoldChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private float _hp;
    public float hp
    {
        get => _hp;
        set
        {
            _hp = Math.Max(value, 0);
            OnHPChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string nickname { get; set; }

    public event EventHandler OnLevelChanged;
    public event EventHandler OnExpChanged;
    public event EventHandler OnGoldChanged;
    public event EventHandler OnHPChanged;
}
