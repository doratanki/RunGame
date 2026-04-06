using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// One rank entry. The player receives this rank when their score is >= minScore.
/// </summary>
[Serializable]
public class RankEntry
{
    public string label;        // "E", "D", "C", "B", "A", "S", "SS", "SSS"
    public int    minScore;
    public Color  color;
    public string animTrigger; // Animator trigger name (rank-up effect)
    public Sprite sprite;      // Image associated with this rank
}

/// <summary>
/// Rank definition table. ScriptableObject — thresholds can be freely adjusted in the Inspector.
/// Create under Assets/Games/StackTower/.
/// </summary>
[CreateAssetMenu(
    menuName = "StackTower/RankTable",
    fileName = "RankTable")]
public class RankTable : ScriptableObject
{
    [Tooltip("Sort entries in ascending order of minScore (E first, SSS last)")]
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

#if UNITY_EDITOR
    void OnValidate()
    {
        for (int i = 1; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i - 1] != null && entries[i].minScore <= entries[i - 1].minScore)
            {
                Debug.LogWarning(
                    $"[RankTable] '{name}': entry [{i}] '{entries[i].label}' (minScore={entries[i].minScore}) " +
                    $"is not greater than entry [{i - 1}] '{entries[i - 1].label}' (minScore={entries[i - 1].minScore}). " +
                    "Entries must be sorted in ascending order of minScore.");
            }
        }
    }
#endif
}
