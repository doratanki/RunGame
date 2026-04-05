using UnityEngine;
using TMPro;

/// <summary>
/// Manages the Stack Tower UI.
/// Assign each panel and text in the Inspector.
/// </summary>
public class TowerUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;

    [Header("In-Game")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public ComboUIAnimator perfectAnimator;
    public ComboUIAnimator goodAnimator;
    public ComboUIAnimator badAnimator;

    [Header("Continue")]
    public ContinueDialog continueDialog;

    [Header("Result")]
    public ResultScreenUI resultScreen;

    [Header("Gallery")]
    public GalleryUI galleryUI;

    [Header("Remove Ads")]
    public RemoveAdsDialog removeAdsDialog;

    public void ShowMenu()
    {
        if (startPanel != null) startPanel.SetActive(true);
        if (gamePanel  != null) gamePanel.SetActive(false);
        continueDialog?.Hide();
        resultScreen?.Hide();
        galleryUI?.Hide();
    }

    public void ShowGame(int score)
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (gamePanel  != null) gamePanel.SetActive(true);
        continueDialog?.Hide();
        resultScreen?.Hide();
        UpdateScore(score);
        if (comboText != null) comboText.text = "";
    }

    public void ShowContinueDialog()
    {
        int score = TowerGameManager.Instance != null ? TowerGameManager.Instance.Score : 0;
        continueDialog?.Show(score);
    }

    public void HideContinueDialog()
    {
        continueDialog?.Hide();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void UpdatePlacement(PlacementQuality quality, int comboCount)
    {
        if (comboText != null)
            comboText.text = comboCount >= 2 ? $"x{comboCount} COMBO" : "";

        switch (quality)
        {
            case PlacementQuality.Perfect:
                perfectAnimator?.PlayPerfect();
                break;
            case PlacementQuality.Good:
                goodAnimator?.PlayGood();
                break;
            case PlacementQuality.Bad:
                badAnimator?.PlayBad();
                break;
        }
    }

    public void ShowGameOver(int score, int best, int perfectCount, int maxCombo)
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (gamePanel  != null) gamePanel.SetActive(false);

        var data = new ResultData
        {
            Score        = score,
            BestScore    = best,
            PerfectCount = perfectCount,
            MaxCombo     = maxCombo,
            IsNewBest    = score >= best,
        };
        resultScreen?.Show(data);
    }

    // ---- Button callbacks ----

    public void OnRemoveAdsButton()
    {
        removeAdsDialog?.Show();
    }

    public void OnGalleryButton()
    {
        galleryUI?.Show();
        if (startPanel != null) startPanel.SetActive(false);
    }

    public void OnGalleryCloseButton()
    {
        galleryUI?.Hide();
        if (startPanel != null) startPanel.SetActive(true);
    }

    public void OnStartButton()
    {
        TowerGameManager.Instance?.StartGame();
    }

    public void OnRestartButton()
    {
        AdsManager.Instance?.ShowInterstitial();
        TowerGameManager.Instance?.RestartGame();
    }

    public void OnBackToTitleButton()
    {
        TowerGameManager.Instance?.BackToTitle();
    }
}
