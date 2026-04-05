using UnityEngine;
using TMPro;

/// <summary>
/// Manages the result screen display.
/// Drives the count-up and rank transition animation via ResultAnimator.
/// </summary>
public class ResultScreenUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI newBestLabel;

    [Header("Rank")]
    public RankDisplayUI rankDisplay;
    public RankTable     rankTable;

    [Header("Animation")]
    public ResultAnimator resultAnimator;

    [Header("Card Lottery")]
    public CardLotteryTable lotteryTable;
    public CardPool         cardPool;
    public CardLotteryUI    cardLotteryUI;

    public void Show(ResultData data)
    {
        if (panel != null) panel.SetActive(true);
        cardLotteryUI?.Hide();

        // Static text updates immediately
        if (bestText     != null) bestText.text     = "BEST       " + data.BestScore;
        if (perfectText  != null) perfectText.text  = "PERFECT    " + data.PerfectCount;
        if (maxComboText != null) maxComboText.text = "MAX COMBO  " + data.MaxCombo;
        if (newBestLabel != null) newBestLabel.gameObject.SetActive(data.IsNewBest);

        // Score and rank are updated via count-up animation
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
            // Fallback without animation
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

        // Clear text to prevent old values flashing before the next Show()
        if (scoreText    != null) scoreText.text    = "";
        if (bestText     != null) bestText.text     = "";
        if (perfectText  != null) perfectText.text  = "";
        if (maxComboText != null) maxComboText.text = "";
        if (newBestLabel != null) newBestLabel.gameObject.SetActive(false);
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
