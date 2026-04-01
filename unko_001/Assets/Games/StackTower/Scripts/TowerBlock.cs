using UnityEngine;

/// <summary>
/// 積み上げ中の1ブロックを管理する。
/// Drop() を呼ぶと移動を止め、前のブロックと重なりを計算して Slice する。
/// </summary>
public class TowerBlock : MonoBehaviour
{
    // BlockSpawner から初期化される
    [HideInInspector] public BlockSpawner spawner;
    [HideInInspector] public TowerBlock previousBlock; // 1つ前のブロック（null = 最初の土台）

    [HideInInspector] public bool isDropped = false;

    // 移動パラメータ（BlockSpawner が設定）
    public float moveSpeed = 3f;
    // +moveRange からスタートするので最初は左向き(-1)
    private int moveDirection = -1;
    private float moveRange = 3f; // X 軸の往復範囲（±）

    // ブロックカラー（BlockSpawner が設定）
    public Color blockColor = Color.white;
    private bool _colorInitialized = false;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // Initialize() で色が明示的に設定された場合のみ適用
        if (_colorInitialized && meshRenderer != null)
        {
            var shader = ShaderUtil.GetLitShader();
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = blockColor;
                meshRenderer.material = mat;
            }
        }
    }

    public void Initialize(float speed, float range, Color color)
    {
        moveSpeed = speed;
        moveRange = range;
        blockColor = color;
        _colorInitialized = true;
    }

    void Update()
    {
        if (isDropped) return;

        // X 軸往復
        transform.position += Vector3.right * moveSpeed * moveDirection * Time.deltaTime;

        float currentX = transform.position.x;
        if (currentX > moveRange || currentX < -moveRange)
        {
            moveDirection *= -1;
            // 範囲内に補正
            float clampedX = Mathf.Clamp(transform.position.x, -moveRange, moveRange);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// タップ/入力時に呼ばれる。ブロックを停止しスライスを実行する。
    /// </summary>
    public void Drop()
    {
        if (isDropped) return;
        isDropped = true;

        if (previousBlock == null)
        {
            // 土台ブロックはそのまま確定
            spawner.OnBlockPlaced(this);
            return;
        }

        Slice();
    }

    void Slice()
    {
        float prevLeft  = previousBlock.transform.position.x - previousBlock.transform.localScale.x / 2f;
        float prevRight = previousBlock.transform.position.x + previousBlock.transform.localScale.x / 2f;
        float currLeft  = transform.position.x - transform.localScale.x / 2f;
        float currRight = transform.position.x + transform.localScale.x / 2f;

        float overlapLeft  = Mathf.Max(prevLeft,  currLeft);
        float overlapRight = Mathf.Min(prevRight, currRight);
        float overlapWidth = overlapRight - overlapLeft;

        if (overlapWidth <= 0f)
        {
            // 完全に外れた → ゲームオーバー
            SpawnDebris(transform.position.x, transform.localScale.x);
            gameObject.SetActive(false);
            TowerGameManager.Instance?.OnGameOver();
            return;
        }

        // はみ出た部分をデブリとして落下
        float leftDiff  = overlapLeft  - currLeft;
        float rightDiff = currRight - overlapRight;

        if (leftDiff > 0.01f)
        {
            float debrisX = currLeft + leftDiff / 2f;
            SpawnDebris(debrisX, leftDiff);
        }
        if (rightDiff > 0.01f)
        {
            float debrisX = overlapRight + rightDiff / 2f;
            SpawnDebris(debrisX, rightDiff);
        }

        // 現ブロックを overlap 幅に切り詰め
        float newCenterX = (overlapLeft + overlapRight) / 2f;
        Vector3 scale = transform.localScale;
        scale.x = overlapWidth;
        transform.localScale = scale;
        transform.position = new Vector3(newCenterX, transform.position.y, transform.position.z);

        spawner.OnBlockPlaced(this);
    }

    void SpawnDebris(float centerX, float width)
    {
        if (width <= 0.01f) return;

        GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debris.name = "Debris";
        debris.transform.position = new Vector3(centerX, transform.position.y, transform.position.z);
        debris.transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);

        // デブリに色を設定
        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = blockColor * 0.7f;
            debris.GetComponent<MeshRenderer>().material = mat;
        }

        // 物理で落下
        Rigidbody rb = debris.AddComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(Random.Range(-1f, 1f), -1f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, Random.Range(-2f, 2f));

        // 5秒後に自動削除
        Destroy(debris, 5f);
    }
}
