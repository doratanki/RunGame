using UnityEngine;

/// <summary>
/// コース上の仲間。未収集時は回転して待機、収集後はプレイヤーの後ろに整列して追従する。
/// </summary>
public class CrowdMember : MonoBehaviour
{
    [Header("見た目")]
    public Color memberColor = new Color(0.2f, 0.8f, 0.3f);

    [Header("待機回転速度")]
    public float idleRotateSpeed = 90f;

    public bool IsCollected { get; private set; } = false;
    private bool _isScattered = false;

    private RunnerPlayer _player;
    private int _index;
    private float _spacing;
    private float _followSpeed;

    void Start()
    {
        ApplyColor(memberColor);
    }

    void Update()
    {
        if (_isScattered) return;

        if (!IsCollected)
        {
            // 待機アニメ：Y 軸回転
            transform.Rotate(0f, idleRotateSpeed * Time.deltaTime, 0f);
            return;
        }

        if (_player == null) return;

        // プレイヤーの後ろ _index 番目の位置に追従
        Vector3 target = _player.transform.position
                         - _player.transform.forward * (_spacing * (_index + 1));
        transform.position = Vector3.Lerp(transform.position, target, _followSpeed * Time.deltaTime);

        // プレイヤーと同じ向きに回転
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            _player.transform.rotation,
            _followSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// プレイヤーが触れたときに呼ばれる。追従を開始する。
    /// </summary>
    public void Collect(RunnerPlayer player, int index)
    {
        IsCollected = true;
        _player    = player;
        _index     = index;
        _spacing   = player.followSpacing;
        _followSpeed = player.followSpeed;

        // コライダーを無効化（二重収集防止）
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        ApplyColor(memberColor);
    }

    /// <summary>
    /// 障害物に当たって仲間が飛ばされるときに呼ばれる。
    /// </summary>
    public void Scatter()
    {
        IsCollected = false;
        _isScattered = true;
        _player = null;

        // Rigidbody を付けてランダム方向に飛ばす
        var rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(2f, 5f),
            Random.Range(-3f, 0f)
        );
        rb.angularVelocity = Random.insideUnitSphere * 5f;

        // 2秒後に削除
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// 整列インデックスを更新する（障害物で前の仲間が消えたとき）。
    /// </summary>
    public void SetIndex(int index)
    {
        _index = index;
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
