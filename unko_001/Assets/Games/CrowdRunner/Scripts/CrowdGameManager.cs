using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Crowd Runner のゲーム状態・仲間数・スコアを管理するシングルトン。
/// </summary>
public class CrowdGameManager : MonoBehaviour
{
    public static CrowdGameManager Instance { get; private set; }

    [Header("References")]
    public RunnerPlayer player;
    public RunnerCamera runnerCamera;
    public CrowdRunnerUI crowdRunnerUI;

    public enum GameState { Menu, Playing, Result }
    public GameState State { get; private set; } = GameState.Menu;

    public int MemberCount { get; private set; } = 0;

    private const string BestScoreKey = "CrowdRunner_BestScore";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        State = GameState.Menu;
        crowdRunnerUI?.ShowMenu();
    }

    public void StartGame()
    {
        MemberCount = 0;
        State = GameState.Playing;

        crowdRunnerUI?.ShowGame(MemberCount);
        player?.StartRunning();

        if (runnerCamera != null && player != null)
            runnerCamera.target = player.transform;
    }

    public void AddMember(int count = 1)
    {
        if (State != GameState.Playing) return;
        MemberCount += count;
        crowdRunnerUI?.UpdateMemberCount(MemberCount);
    }

    public void RemoveMember(int count)
    {
        if (State != GameState.Playing) return;
        MemberCount = Mathf.Max(0, MemberCount - count);
        crowdRunnerUI?.UpdateMemberCount(MemberCount);
    }

    public void OnResult(bool isWin)
    {
        if (State != GameState.Playing) return;
        State = GameState.Result;

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (MemberCount > best)
        {
            best = MemberCount;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        crowdRunnerUI?.ShowResult(isWin, MemberCount, best);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("GameSelect");
    }
}
