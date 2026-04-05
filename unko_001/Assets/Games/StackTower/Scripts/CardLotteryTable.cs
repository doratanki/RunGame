using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// レアリティと重みのペア。
/// </summary>
[Serializable]
public class RarityRate
{
    public CardRarity rarity;
    [Range(0f, 100f)] public float weight;
}

/// <summary>
/// ランク1つ分の抽選テーブル。
/// </summary>
[Serializable]
public class RankLotteryEntry
{
    [Tooltip("RankEntry.label と一致させること（例: S）")]
    public string rankLabel;
    public List<RarityRate> rates = new();
}

/// <summary>
/// ランク別カード抽選テーブル。ScriptableObject で管理する。
/// デフォルト値はあくまで参考値。Inspector で調整すること。
/// </summary>
[CreateAssetMenu(menuName = "StackTower/CardLotteryTable", fileName = "CardLotteryTable")]
public class CardLotteryTable : ScriptableObject
{
    [Tooltip("ランクラベル昇順で並べること（E が先頭、SSS が末尾）")]
    public List<RankLotteryEntry> entries = new()
    {
        new RankLotteryEntry { rankLabel = "E",   rates = new() {
            new() { rarity = CardRarity.Common, weight = 97 },
            new() { rarity = CardRarity.Rare,   weight = 3  },
        }},
        new RankLotteryEntry { rankLabel = "D",   rates = new() {
            new() { rarity = CardRarity.Common, weight = 90 },
            new() { rarity = CardRarity.Rare,   weight = 10 },
        }},
        new RankLotteryEntry { rankLabel = "C",   rates = new() {
            new() { rarity = CardRarity.Common, weight = 75 },
            new() { rarity = CardRarity.Rare,   weight = 23 },
            new() { rarity = CardRarity.Epic,   weight = 2  },
        }},
        new RankLotteryEntry { rankLabel = "B",   rates = new() {
            new() { rarity = CardRarity.Common, weight = 60 },
            new() { rarity = CardRarity.Rare,   weight = 33 },
            new() { rarity = CardRarity.Epic,   weight = 7  },
        }},
        new RankLotteryEntry { rankLabel = "A",   rates = new() {
            new() { rarity = CardRarity.Common,    weight = 40 },
            new() { rarity = CardRarity.Rare,      weight = 40 },
            new() { rarity = CardRarity.Epic,      weight = 18 },
            new() { rarity = CardRarity.Legendary, weight = 2  },
        }},
        new RankLotteryEntry { rankLabel = "S",   rates = new() {
            new() { rarity = CardRarity.Rare,      weight = 50 },
            new() { rarity = CardRarity.Epic,      weight = 40 },
            new() { rarity = CardRarity.Legendary, weight = 10 },
        }},
        new RankLotteryEntry { rankLabel = "SS",  rates = new() {
            new() { rarity = CardRarity.Rare,      weight = 25 },
            new() { rarity = CardRarity.Epic,      weight = 50 },
            new() { rarity = CardRarity.Legendary, weight = 25 },
        }},
        new RankLotteryEntry { rankLabel = "SSS", rates = new() {
            new() { rarity = CardRarity.Epic,      weight = 30 },
            new() { rarity = CardRarity.Legendary, weight = 70 },
        }},
    };

    public RankLotteryEntry GetEntry(string rankLabel) =>
        entries.Find(e => e.rankLabel == rankLabel);
}
