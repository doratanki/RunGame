using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ランクのビジュアル表示・アニメーションを担当する。
/// Animator は省略可能。アサインすれば animTrigger で演出が走る。
/// </summary>
public class RankDisplayUI : MonoBehaviour
{
    [Header("テキスト")]
    public TextMeshProUGUI rankLabel;

    [Header("画像")]
    public Image rankImage;

    [Header("アニメーション（省略可）")]
    public Animator animator;

    private RankEntry _current;

    /// <summary>
    /// 即時反映（演出なし）。初期表示などに使う。
    /// </summary>
    public void SetRank(RankEntry rank)
    {
        _current = rank;
        ApplyVisual(rank);
    }

    /// <summary>
    /// ランクアップ演出付きで更新する。
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
