using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController player;
    public CameraFollow cameraFollow;
    public LevelGenerator levelGenerator;

    [Header("UI")]
    public GameObject startPanel;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    public enum GameState { Menu, Playing, Ended }
    public GameState state = GameState.Menu;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ShowMenu();
    }

    void ShowMenu()
    {
        state = GameState.Menu;
        if (startPanel != null) startPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    public void StartGame()
    {
        state = GameState.Playing;
        if (startPanel != null) startPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        // Setup player
        player.transform.position = Vector3.zero;
        player.currentNumber = 1f;
        player.UpdateNumberDisplay();

        // Setup camera
        cameraFollow.target = player.transform;

        // Generate level
        levelGenerator.GenerateLevel();

        // Start running
        player.StartRunning();
    }

    public void OnWin()
    {
        state = GameState.Ended;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "YOU WIN!";
    }

    public void OnLose()
    {
        state = GameState.Ended;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "YOU LOSE...";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
