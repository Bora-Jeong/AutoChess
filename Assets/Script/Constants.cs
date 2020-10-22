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
public static class Constants
{
    public static readonly float prepareTimeSpan = 30f;
    public static readonly float waitTimeSpan = 5f;
    public static readonly float battleTimeSpan = 30f;

    public static readonly int playerMaxLevel = 10; // 플레이어 최대 레벨
    public static readonly int unitMaxGold = 5; // 유닛 최대 골드
    public static readonly int goldToShuffleShop = 2; // 상점 셔플을 위해 필요한 골드
    public static readonly float unitScaleIncreaseAmount = 0.3f;  // 1성 1 , 2성 1.3, 3성 1.6

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
}