using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランク1エントリー。minScore 以上のスコアでこのランクになる。
/// </summary>
[Serializable]
public class RankEntry
{
    public string label;        // "E", "D", "C", "B", "A", "S", "SS", "SSS"
    public int    minScore;
    public Color  color;
    public string animTrigger; // Animator トリガー名（ランクアップ演出）
    public Sprite sprite;      // ランクに対応する画像
}

/// <summary>
/// ランク定義テーブル。ScriptableObject なので Inspector で閾値を自由に調整できる。
/// Assets/Games/StackTower/ 以下に作成すること。
/// </summary>
[CreateAssetMenu(
    menuName = "StackTower/RankTable",
    fileName = "RankTable")]
public class RankTable : ScriptableObject
{
    [Tooltip("minScore の昇順で並べること（E が先頭、SSS が末尾）")]
    public List<RankEntry> entries = new()
    {
        new RankEntry { label = "E",   minScore = 0,    color = new Color(0.5f, 0.5f, 0.5f), animTrigger = "RankE"   },
        new RankEntry { label = "D",   minScore = 30,   color = new Color(0.6f, 0.4f, 0.2f), animTrigger = "RankD"   },
        new RankEntry { label = "C",   minScore = 60,   color = new Color(0.2f, 0.7f, 0.2f), animTrigger = "RankC"   },
        new RankEntry { label = "B",   minScore = 100,  color = new Color(0.2f, 0.5f, 0.9f), animTrigger = "RankB"   },
        new RankEntry { label = "A",   minScore = 150,  color = new Color(0.6f, 0.2f, 0.9f), animTrigger = "RankA"   },
        new RankEntry { label = "S",   minScore = 220,  color = new Color(1.0f, 0.8f, 0.1f), animTrigger = "RankS"   },
        new RankEntry { label = "SS",  minScore = 300,  color = new Color(1.0f, 0.5f, 0.0f), animTrigger = "RankSS"  },
        new RankEntry { label = "SSS", minScore = 400,  color = new Color(1.0f, 0.2f, 0.2f), animTrigger = "RankSSS" },
    };
}
