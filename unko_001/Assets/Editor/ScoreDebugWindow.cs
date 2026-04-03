#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// スコア関連の PlayerPrefs を操作するエディターウィンドウ。
/// メニュー: Tools > Score Debug
/// </summary>
public class ScoreDebugWindow : EditorWindow
{
    private const string BestScoreKey = "TexasMeatTower_BestScore";
    private int _newBestScore;

    [MenuItem("Tools/Score Debug")]
    public static void ShowWindow()
    {
        GetWindow<ScoreDebugWindow>("Score Debug");
    }

    void OnGUI()
    {
        GUILayout.Label("スコアデバッグ", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        int current = PlayerPrefs.GetInt(BestScoreKey, 0);
        EditorGUILayout.LabelField("現在のベストスコア", current.ToString());

        EditorGUILayout.Space();

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("ベストスコアをリセット", GUILayout.Height(36)))
        {
            PlayerPrefs.DeleteKey(BestScoreKey);
            PlayerPrefs.Save();
            Debug.Log("[ScoreDebug] ベストスコアをリセットしました。");
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();
        GUILayout.Label("ベストスコアを上書き", EditorStyles.boldLabel);
        _newBestScore = EditorGUILayout.IntField("新しいスコア", _newBestScore);

        if (GUILayout.Button("上書き保存", GUILayout.Height(36)))
        {
            PlayerPrefs.SetInt(BestScoreKey, _newBestScore);
            PlayerPrefs.Save();
            Debug.Log($"[ScoreDebug] ベストスコアを {_newBestScore} に設定しました。");
        }
    }
}
#endif
