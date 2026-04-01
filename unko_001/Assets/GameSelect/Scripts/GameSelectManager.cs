using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム選択画面のマネージャー。
/// 各ゲームのシーン名は Build Settings に登録してください。
/// </summary>
public class GameSelectManager : MonoBehaviour
{
    public void LoadBallBounce()
    {
        SceneManager.LoadScene("BallBounce");
    }

    public void LoadCrowdRunner()
    {
        SceneManager.LoadScene("CrowdRunner");
    }

    public void LoadStackTower()
    {
        SceneManager.LoadScene("StackTower");
    }
}
