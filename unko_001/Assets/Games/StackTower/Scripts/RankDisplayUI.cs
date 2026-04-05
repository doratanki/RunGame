using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the visual display and animation of the rank.
/// Animator is optional; if assigned, animTrigger fires the rank-up effect.
/// </summary>
public class RankDisplayUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI rankLabel;

    [Header("Image")]
    public Image rankImage;

    [Header("Animation (optional)")]
    public Animator animator;

    private RankEntry _current;

    /// <summary>
    /// Immediate update without animation. Use for initial display.
    /// </summary>
    public void SetRank(RankEntry rank)
    {
        _current = rank;
        ApplyVisual(rank);
    }

    /// <summary>
    /// Updates with rank-up animation.
    /// </summary>
    public void PlayRankUp(RankEntry rank)
    {
        _current = rank;
        ApplyVisual(rank);

        if (animator != null && !string.IsNullOrEmpty(rank.animTrigger))
            animator.SetTrigger(rank.animTrigger);
    }

    public RankEntry Current => _current;

    void ApplyVisual(RankEntry rank)
    {
        if (rank == null) return;

        if (rankLabel != null)
        {
            rankLabel.text  = rank.label;
            rankLabel.color = rank.color;
        }

        if (rankImage != null)
        {
            rankImage.sprite  = rank.sprite;
            rankImage.enabled = rank.sprite != null;
        }
    }
}
