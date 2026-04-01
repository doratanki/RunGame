using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ball Bounce のゲーム状態・スコアを管理するシングルトン。
/// </summary>
public class BallGameManager : MonoBehaviour
{
    public static BallGameManager Instance { get; private set; }

    [Header("References")]
    public BallController ballController;
    public TileSpawner tileSpawner;
    public BallGameUI ballGameUI;

    public enum GameState { Menu, Playing, GameOver }
    public GameState State { get; private set; } = GameState.Menu;

    public int Score { get; private set; } = 0;

    private const string BestScoreKey = "BallBounce_BestScore";

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
        ballGameUI?.ShowMenu();
    }

    public void StartGame()
    {
        Score = 0;
        State = GameState.Playing;

        ballGameUI?.ShowGame(Score);
        tileSpawner?.StartSpawning(ballController?.transform);
        ballController?.StartBall();
    }

    public void AddScore()
    {
        if (State != GameState.Playing) return;
        Score++;
        ballGameUI?.UpdateScore(Score);
    }

    public void OnGameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;

        ballController?.StopBall();

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (Score > best)
        {
            best = Score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        ballGameUI?.ShowGameOver(Score, best);
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
