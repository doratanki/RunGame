using UnityEngine;
using TMPro;

/// <summary>
/// リザルト画面の表示を管理するクラス。
/// ResultAnimator でカウントアップ・ランク遷移演出を駆動する。
/// </summary>
public class ResultScreenUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("テキスト")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI newBestLabel;

    [Header("ランク")]
    public RankDisplayUI rankDisplay;
    public RankTable     rankTable;

    [Header("演出")]
    public ResultAnimator resultAnimator;

    [Header("カード抽選")]
    public CardLotteryTable lotteryTable;
    public CardPool         cardPool;
    public CardLotteryUI    cardLotteryUI;

    public void Show(ResultData data)
    {
        if (panel != null) panel.SetActive(true);
        cardLotteryUI?.Hide();

        // 静的テキストは即時反映
        if (bestText     != null) bestText.text     = "BEST       " + data.BestScore;
        if (perfectText  != null) perfectText.text  = "PERFECT    " + data.PerfectCount;
        if (maxComboText != null) maxComboText.text = "MAX COMBO  " + data.MaxCombo;
        if (newBestLabel != null) newBestLabel.gameObject.SetActive(data.IsNewBest);

        // スコア＆ランクはカウントアップ演出で更新
        if (resultAnimator != null && rankTable != null)
        {
            resultAnimator.Animate(
                data.Score,
                rankTable,
                score => { if (scoreText != null) scoreText.text = "SCORE      " + score; },
                rank  => rankDisplay?.PlayRankUp(rank),
                ()    => OnAnimationComplete(data)
            );
        }
        else
        {
            // 演出なしのフォールバック
            if (scoreText != null) scoreText.text = "SCORE      " + data.Score;
            RankEntry rank = RankCalculator.GetRank(rankTable, data.Score);
            if (rank != null) rankDisplay?.SetRank(rank);
            OnAnimationComplete(data);
        }
    }

    public void Hide()
    {
        resultAnimator?.Stop();
        cardLotteryUI?.Hide();
        if (panel != null) panel.SetActive(false);
    }

    void OnAnimationComplete(ResultData data)
    {
        if (lotteryTable == null || cardPool == null || cardLotteryUI == null) return;

        RankEntry finalRank = RankCalculator.GetRank(rankTable, data.Score);
        if (finalRank == null) return;

        CardLotteryResult result = CardLottery.Draw(lotteryTable, cardPool, finalRank.label);
        cardLotteryUI.Show(result);
    }
}
