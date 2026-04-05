using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ランクギャラリーの1セル。
/// ランク画像・ラベル・スコア条件を表示する。
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
                ? "〜 " + NextMinScore(entry) + " 未満"
                : entry.minScore + " 〜";
    }

    // minScoreText の上限表示用。GalleryUI 側から注入する方式にしても良い。
    // ここでは Setup 後に呼ぶ別メソッドで対応。
    public void SetScoreRange(int min, int? max)
    {
        if (minScoreText == null) return;
        minScoreText.text = max.HasValue
            ? $"{min} 〜 {max - 1}"
            : $"{min} 〜";
    }

    static int NextMinScore(RankEntry _) => 0; // ダミー。GalleryUI 側で SetScoreRange を使う
}
