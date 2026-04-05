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

        // カメラターゲットを BlockSpawner の topBlockTransform にセット
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

        // コンティニューがまだ使われていなければ 30% の確率でダイアログを出す
        if (!HasContinued && Random.value < 0.3f)
        {
            towerUI?.ShowContinueDialog();
            return;
        }

        ShowResult();
    }

    /// <summary>コンティニューダイアログで「続ける」を選んだ時に呼ぶ。</summary>
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

    /// <summary>コンティニューダイアログで「あきらめる」を選んだ時に呼ぶ。</summary>
    public void GiveUp()
    {
        HasContinued = true; // 使用済みにして直接リザルトへ
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
        // カメラを即座に初期位置へスナップ（前のゲームの高さに残らないよう）
        cameraFollow?.SnapToTarget();
    }

    public void BackToTitle()
    {
        blockSpawner?.ClearBlocks();
        State = GameState.Menu;
        towerUI?.ShowMenu();
    }
}
