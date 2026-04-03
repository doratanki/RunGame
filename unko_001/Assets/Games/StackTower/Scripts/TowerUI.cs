using UnityEngine;
using TMPro;

/// <summary>
/// Stack Tower の UI を管理する。
/// Inspector で各パネル・テキストをアサインする。
/// </summary>
public class TowerUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    [Header("ゲーム中")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public ComboUIAnimator perfectAnimator;
    public ComboUIAnimator goodAnimator;
    public ComboUIAnimator badAnimator;

    [Header("ゲームオーバー")]
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestText;

    public void ShowMenu()
    {
        SetPanels(start: true, game: false, gameOver: false);
    }

    public void ShowGame(int score)
    {
        SetPanels(start: false, game: true, gameOver: false);
        UpdateScore(score);
        if (comboText != null) comboText.text = "";
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

    public void ShowGameOver(int score, int best)
    {
        SetPanels(start: false, game: false, gameOver: true);

        if (gameOverScoreText != null)
            gameOverScoreText.text = "SCORE  " + score;

        if (gameOverBestText != null)
            gameOverBestText.text = "BEST   " + best;
    }

    void SetPanels(bool start, bool game, bool gameOver)
    {
        if (startPanel   != null) startPanel.SetActive(start);
        if (gamePanel    != null) gamePanel.SetActive(game);
        if (gameOverPanel != null) gameOverPanel.SetActive(gameOver);
    }

    // ---- ボタンから呼ぶメソッド ----

    public void OnStartButton()
    {
        TowerGameManager.Instance?.StartGame();
    }

    public void OnRestartButton()
    {
        TowerGameManager.Instance?.RestartGame();
    }

    public void OnBackToTitleButton()
    {
        TowerGameManager.Instance?.BackToTitle();
    }
}
