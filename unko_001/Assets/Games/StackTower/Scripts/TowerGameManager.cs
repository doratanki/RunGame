using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that manages game state and score for Stack Tower.
/// </summary>
public class TowerGameManager : Singleton<TowerGameManager>
{
    [Header("Config")]
    public GameConfig gameConfig;

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

        int comboCap = gameConfig != null ? gameConfig.maxComboCap : 5;
        int bonus = quality == PlacementQuality.Perfect ? Mathf.Min(ComboCount, comboCap) : 0;
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

        // Show continue dialog with configurable chance if not already used
        float continueChance = gameConfig != null ? gameConfig.continueChance : 0.3f;
        if (!HasContinued && Random.value < continueChance)
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
        int best = LoadBestScore();
        if (Score > best)
        {
            best = Score;
            SaveBestScore(best);
        }

        towerUI?.ShowGameOver(Score, best, PerfectCount, MaxCombo);
    }

    static int LoadBestScore()
    {
        try
        {
            return PlayerPrefs.GetInt(BestScoreKey, 0);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[TowerGameManager] Failed to load best score: {e.Message}");
            return 0;
        }
    }

    static void SaveBestScore(int score)
    {
        try
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[TowerGameManager] Failed to save best score: {e.Message}");
        }
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
