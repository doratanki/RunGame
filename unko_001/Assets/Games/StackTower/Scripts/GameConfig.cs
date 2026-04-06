using UnityEngine;

/// <summary>
/// Game balance parameters for Stack Tower.
/// Create via: Assets > Create > StackTower > GameConfig
/// Assign the asset to TowerGameManager in the Inspector.
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "StackTower/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Placement Thresholds")]
    [Tooltip("Offset smaller than this is treated as Perfect (in world units).")]
    public float perfectThreshold = 0.1f;

    [Tooltip("Trim-to-size ratio below this is treated as Good (e.g. 0.35 = 35% of block width).")]
    [Range(0f, 1f)] public float goodRatio = 0.35f;

    [Tooltip("First block above foundation gets its hit area widened by this multiplier.")]
    public float foundationHitMultiplier = 1.7f;

    [Header("Scoring")]
    [Tooltip("Maximum combo count added as bonus score per Perfect placement.")]
    public int maxComboCap = 5;

    [Header("Continue")]
    [Tooltip("Probability (0–1) that the continue dialog is shown after game over.")]
    [Range(0f, 1f)] public float continueChance = 0.3f;
}
