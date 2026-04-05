using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that manages game state and score for Stack Tower.
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
    public int PerfectCount { get; private set; } = 0;
    public int MaxCombo { get; private set; } = 0;

    public bool HasContinued { get; private set; } = false;

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
        PerfectCount = 0;
        MaxCombo = 0;
        HasContinued = false;
        State = GameState.Playing;
        towerUI?.ShowGame(Score);
        blockSpawner?.StartSpawning();

        // Set camera target to BlockSpawner's topBlockTransform
        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    public void OnBlockStacked(PlacementQuality quality = PlacementQuality.Good)
    {
        if (State != GameState.Playing) return;

        if (quality == PlacementQuality.Perfect)
        {
            ComboCount++;
            PerfectCount++;
            if (ComboCount > MaxCombo) MaxCombo = ComboCount;
        }
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

        // Update camera target to the latest block
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

        // Show continue dialog with 30% chance if not already used
        if (!HasContinued && Random.value < 0.3f)
        {
            towerUI?.ShowContinueDialog();
            return;
        }

        ShowResult();
    }

    /// <summary>Call when the player chooses to continue from the continue dialog.</summary>
    public void ContinueGame()
    {
        if (HasContinued) return;
        HasContinued = true;
        State = GameState.Playing;

        towerUI?.HideContinueDialog();
        blockSpawner?.ContinueSpawning();

        if (cameraFollow != null && blockSpawner != null)
            cameraFollow.target = blockSpawner.topBlockTransform;
    }

    /// <summary>Call when the player chooses to give up from the continue dialog.</summary>
    public void GiveUp()
    {
        HasContinued = true; // Mark as used and go directly to result
        ShowResult();
    }

    void ShowResult()
    {
        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (Score > best)
        {
            best = Score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        towerUI?.ShowGameOver(Score, best, PerfectCount, MaxCombo);
    }

    public void RestartGame()
    {
        blockSpawner?.ClearBlocks();
        StartGame();
        // Snap camera to initial position immediately so it doesn't stay at the previous game's height
        cameraFollow?.SnapToTarget();
    }

    public void BackToTitle()
    {
        blockSpawner?.ClearBlocks();
        State = GameState.Menu;
        towerUI?.ShowMenu();
    }
}
