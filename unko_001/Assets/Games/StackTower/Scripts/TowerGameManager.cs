using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stack Tower のゲーム状態・スコアを管理するシングルトン。
/// </summary>
public class TowerGameManager : MonoBehaviour
{
    public static TowerGameManager Instance { get; private set; }

    [Header("References")]
    public BlockSpawner blockSpawner;
    public TowerUI towerUI;
    public CameraFollow cameraFollow;

    public enum GameState { Menu, Playing, GameOver }
    public GameState State { get; private set; } = GameState.Menu;

    public int Score { get; private set; } = 0;

    private const string BestScoreKey = "StackTower_BestScore";

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
        towerUI?.ShowMenu();
    }

    public void StartGame()
    {
        Score = 0;
        State = GameState.Playing;

        towerUI?.ShowGame(Score);
        blockSpawner?.StartSpawning();

        // カメラターゲットを BlockSpawner の topBlockTransform にセット
        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    public void OnBlockStacked()
    {
        if (State != GameState.Playing) return;

        Score++;
        towerUI?.UpdateScore(Score);

        // カメラターゲットを最新ブロックに更新
        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    public void OnGameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;

        blockSpawner?.StopSpawning();

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (Score > best)
        {
            best = Score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        towerUI?.ShowGameOver(Score, best);
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
