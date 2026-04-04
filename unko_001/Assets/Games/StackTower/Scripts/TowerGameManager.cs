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
    public int ComboCount { get; private set; } = 0;

    private const string BestScoreKey = "TexasMeatTower_BestScore";

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
        TowerAudioManager.Instance?.PlayBGM();
        towerUI?.ShowMenu();
    }

    public void StartGame()
    {
        Score = 0;
        ComboCount = 0;
        State = GameState.Playing;
        towerUI?.ShowGame(Score);
        blockSpawner?.StartSpawning();

        // カメラターゲットを BlockSpawner の topBlockTransform にセット
        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    public void OnBlockStacked(PlacementQuality quality = PlacementQuality.Good)
    {
        if (State != GameState.Playing) return;

        if (quality == PlacementQuality.Perfect)
            ComboCount++;
        else
            ComboCount = 0;

        int bonus = quality == PlacementQuality.Perfect ? Mathf.Min(ComboCount, 5) : 0;
        Score += 1 + bonus;

        if (quality == PlacementQuality.Perfect)
            TowerAudioManager.Instance?.PlayPerfect();
        else
            TowerAudioManager.Instance?.PlayBlockPlace();

        towerUI?.UpdateScore(Score);
        towerUI?.UpdatePlacement(quality, ComboCount);

        // カメラターゲットを最新ブロックに更新
        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    public void OnGameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;

        ComboCount = 0;
        TowerAudioManager.Instance?.PlayGameOver();
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
        blockSpawner?.ClearBlocks();
        StartGame();
    }

    public void BackToTitle()
    {
        blockSpawner?.ClearBlocks();
        State = GameState.Menu;
        towerUI?.ShowMenu();
    }
}
