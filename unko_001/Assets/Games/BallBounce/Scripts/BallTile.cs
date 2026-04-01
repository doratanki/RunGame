using UnityEngine;

/// <summary>
/// コース上の1タイル。ボールが乗ったら消滅してスコアを加算する。
/// TileSpawner から Initialize() で色を受け取る。
/// </summary>
public class BallTile : MonoBehaviour
{
    public Color tileColor = Color.white;

    private bool _bounced = false;

    public void Initialize(Color color)
    {
        tileColor = color;
        ApplyColor(color);
    }

    /// <summary>
    /// BallController の OnCollisionEnter から呼ばれる。
    /// </summary>
    public void OnBounced()
    {
        if (_bounced) return;
        _bounced = true;

        BallGameManager.Instance?.AddScore();
        Destroy(gameObject);
    }

    void ApplyColor(Color color)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = color;
            renderer.material = mat;
        }
    }
}
