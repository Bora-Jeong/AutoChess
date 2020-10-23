using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Clas
{
    Devil,
    Ork,
    Dragon,
    Human,
    Elf
}

public enum Species
{
    Knight,
    Mage,
    Hunter,
    Assassin,
    Warrior
}
public enum DamageType
{
    Physics,
    Magic
}

public static class Constants
{
    // Game State 지속 시간
    public static readonly float prepareTimeSpan = 10f; // debug용 원래 30초
    public static readonly float waitTimeSpan = 3f;
    public static readonly float battleTimeSpan = 10f;  // debug용 원래 30초
    public static readonly float resultTimeSpan = 3f;

    public static readonly int playerMaxLevel = 10; // 플레이어 최대 레벨
    public static readonly int unitMaxGold = 5; // 유닛 최대 골드
    public static readonly int requiredGoldToShuffleShop = 2; // 상점 셔플을 위해 필요한 골드
    public static readonly float unitScaleIncreaseAmount = 0.3f;  // 1성 1 , 2성 1.3, 3성 1.6

    public static int[] unitSpawnPercentInShop =
    {
        100, 0, 0, 0, 0, // 플레이어 1레벨
        70, 30, 0, 0, 0, // 플레이어 2레벨
        60, 35, 5, 0, 0, // 플레이어 3레벨
        50, 35, 15, 0, 0, // 플레이어 4레벨
        40, 35, 23, 2, 0, // 플레이어 5레벨
        33, 30, 30, 7, 0, // 플레이어 6레벨
        30, 30, 30, 10, 0, // 플레이어 7레벨
        24, 30, 30, 15, 1, // 플레이어 8레벨
        22, 30, 25, 20, 3, // 플레이어 9레벨
        19, 25, 25, 25, 6 // 플레이어 10레벨
    };

    public static string ToName(this Clas cls)
    {
        switch (cls)
        {
            case Clas.Devil:
                return "악마";
            case Clas.Ork:
                return "오크";
            case Clas.Dragon:
                return "용";
            case Clas.Human:
                return "인간";
            case Clas.Elf:
                return "엘프";
            default:
                return string.Empty;
        }
    }

    public static string ToName(this Species spc)
    {
        switch (spc)
        {
            case Species.Knight:
                return "기사";
            case Species.Mage:
                return "마법사";
            case Species.Hunter:
                return "사냥꾼";
            case Species.Assassin:
                return "암살자";
            case Species.Warrior:
                return "전사";
            default:
                return string.Empty;
        }
    }

    public static string ToName(this DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physics:
                return "물리";
            case DamageType.Magic:
                return "마법";
            default:
                return string.Empty;
        }
    }

    public static string Convert(this string str) => TableData.instance.GetStringTableData(str);
}