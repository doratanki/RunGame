using UnityEngine;

/// <summary>
/// ゴール地点の敵ゲート。プレイヤーが触れたら仲間数と敵数を比較して勝敗を決める。
/// </summary>
public class EnemyGate : MonoBehaviour
{
    [Header("敵の人数（Inspector で設定）")]
    public int enemyCount = 10;

    [Header("見た目")]
    public Color enemyColor = new Color(0.3f, 0.3f, 0.3f);

    private bool _triggered = false;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = enemyColor;
            renderer.material = mat;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        RunnerPlayer player = other.GetComponent<RunnerPlayer>();
        if (player == null) return;

        _triggered = true;
        player.StopRunning();

        bool isWin = CrowdGameManager.Instance != null
                     && CrowdGameManager.Instance.MemberCount > enemyCount;

        CrowdGameManager.Instance?.OnResult(isWin);
    }
}
