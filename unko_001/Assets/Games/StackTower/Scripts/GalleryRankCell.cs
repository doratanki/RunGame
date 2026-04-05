using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One cell in the rank gallery.
/// Displays the rank image, label, and score requirement.
/// </summary>
public class GalleryRankCell : MonoBehaviour
{
    public Image           rankImage;
    public TextMeshProUGUI rankLabel;
    public TextMeshProUGUI minScoreText;

    public void Setup(RankEntry entry)
    {
        if (rankImage != null)
        {
            rankImage.sprite  = entry.sprite;
            rankImage.color   = entry.color;
            rankImage.enabled = entry.sprite != null;
        }

        if (rankLabel != null)
        {
            rankLabel.text  = entry.label;
            rankLabel.color = entry.color;
        }

        if (minScoreText != null)
            minScoreText.text = entry.minScore == 0
                ? "0 ~ " + NextMinScore(entry)
                : entry.minScore + " ~";
    }

    public void SetScoreRange(int min, int? max)
    {
        if (minScoreText == null) return;
        minScoreText.text = max.HasValue
            ? $"{min} ~ {max - 1}"
            : $"{min} ~";
    }

    static int NextMinScore(RankEntry _) => 0; // Dummy. GalleryUI uses SetScoreRange instead.
}
