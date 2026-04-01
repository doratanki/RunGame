using UnityEngine;

/// <summary>
/// 障害物。プレイヤーが触れると仲間を lossCount 人減らす。
/// Inspector で lossCount と色を設定する。
/// </summary>
public class Obstacle : MonoBehaviour
{
    [Header("設定")]
    public int lossCount = 3;
    public Color obstacleColor = new Color(0.9f, 0.2f, 0.2f);

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = obstacleColor;
            renderer.material = mat;
        }
    }
}
