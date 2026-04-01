using UnityEngine;
using TMPro;

/// <summary>
/// Crowd Runner の UI を管理する。
/// Inspector で各パネル・テキストをアサインする。
/// </summary>
public class CrowdRunnerUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    [Header("ゲーム中")]
    public TextMeshProUGUI memberCountText;

    [Header("リザルト")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI resultBestText;

    public void ShowMenu()
    {
        SetPanels(start: true, game: false, result: false);
    }

    public void ShowGame(int memberCount)
    {
        SetPanels(start: false, game: true, result: false);
        UpdateMemberCount(memberCount);
    }

    public void UpdateMemberCount(int count)
    {
        if (memberCountText != null)
            memberCountText.text = count.ToString();
    }

    public void ShowResult(bool isWin, int score, int best)
    {
        SetPanels(start: false, game: false, result: true);

        if (resultText != null)
            resultText.text = isWin ? "WIN!" : "LOSE...";

        if (resultScoreText != null)
            resultScoreText.text = "SCORE  " + score;

        if (resultBestText != null)
            resultBestText.text = "BEST   " + best;
    }

    void SetPanels(bool start, bool game, bool result)
    {
        if (startPanel  != null) startPanel.SetActive(start);
        if (gamePanel   != null) gamePanel.SetActive(game);
        if (resultPanel != null) resultPanel.SetActive(result);
    }

    // ---- ボタンから呼ぶメソッド ----

    public void OnStartButton()
    {
        CrowdGameManager.Instance?.StartGame();
    }

    public void OnRestartButton()
    {
        CrowdGameManager.Instance?.RestartGame();
    }
}
