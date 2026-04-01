using UnityEngine;

/// <summary>
/// ボールの物理移動・タップ入力・カメラ追従・死亡判定を管理する。
/// Rigidbody が必須コンポーネント。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("移動")]
    public float forwardSpeed  = 6f;    // 右方向への自動速度
    public float holdForce     = 18f;   // ホールド中に加える上昇力
    public float maxUpVelocity = 8f;    // 上昇速度の上限

    [Header("死亡ライン")]
    public float deathY = -6f;          // これ以下に落ちたらゲームオーバー

    [Header("カメラ追従")]
    public Vector3 cameraOffset    = new Vector3(-4f, 3f, -10f);
    public float   cameraSmoothSpeed = 5f;

    [Header("見た目")]
    public Color ballColor = new Color(1f, 0.4f, 0.1f);

    private Rigidbody _rb;
    private Camera _mainCamera;
    private bool _isPlaying = false;
    private bool _isDead = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Z 軸固定（2D 的な動き）
        _rb.constraints = RigidbodyConstraints.FreezePositionZ
                        | RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        _mainCamera = Camera.main;
        ApplyColor();
    }

    public void StartBall()
    {
        _isPlaying = true;
        _isDead = false;
    }

    public void StopBall()
    {
        _isPlaying = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;
    }

    void Update()
    {
        if (!_isPlaying || _isDead) return;

        // 死亡判定
        if (transform.position.y < deathY)
        {
            _isDead = true;
            BallGameManager.Instance?.OnGameOver();
            return;
        }
    }

    void FixedUpdate()
    {
        if (!_isPlaying || _isDead) return;

        // 右方向速度を常に維持
        Vector3 vel = _rb.linearVelocity;
        vel.x = forwardSpeed;
        _rb.linearVelocity = vel;

        // タップ / ホールド中に上昇力を加える
        bool holding = Input.GetMouseButton(0) || (Input.touchCount > 0);
        if (holding && _rb.linearVelocity.y < maxUpVelocity)
        {
            _rb.AddForce(Vector3.up * holdForce, ForceMode.Acceleration);
        }
    }

    void LateUpdate()
    {
        if (!_isPlaying) return;

        // カメラをボールに追従させる
        if (_mainCamera != null)
        {
            Vector3 desired = transform.position + cameraOffset;
            _mainCamera.transform.position = Vector3.Lerp(
                _mainCamera.transform.position,
                desired,
                cameraSmoothSpeed * Time.deltaTime
            );
            _mainCamera.transform.LookAt(transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        BallTile tile = collision.gameObject.GetComponent<BallTile>();
        if (tile != null)
        {
            // タイル上面からの衝突のみ反応（横や底面は無視）
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    tile.OnBounced();
                    break;
                }
            }
        }
    }

    void ApplyColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = ballColor;
            renderer.material = mat;
        }
    }
}
