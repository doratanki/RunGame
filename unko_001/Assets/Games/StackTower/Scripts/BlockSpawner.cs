using UnityEngine;

/// <summary>
/// ブロックの生成・移動・入力受付を管理する。
/// TowerGameManager から StartSpawning() を呼んでゲームを開始する。
/// </summary>
public class BlockSpawner : MonoBehaviour
{
    [Header("ブロック設定")]
    public float blockWidth  = 3f;
    public float blockHeight = 0.4f;
    public float blockDepth  = 3f;
    public float moveRange   = 3f;  // X 軸の往復範囲（±）

    [Header("難易度")]
    public float baseSpeed      = 2.5f;
    public float speedIncrement = 0.1f;  // スコアごとの加速量

    [Header("カラーパレット（Inspector で設定）")]
    public Color[] blockColors = new Color[]
    {
        new Color(0.95f, 0.35f, 0.35f),
        new Color(0.35f, 0.75f, 0.95f),
        new Color(0.35f, 0.95f, 0.55f),
        new Color(0.95f, 0.85f, 0.25f),
        new Color(0.85f, 0.45f, 0.95f),
    };

    // カメラが追従するための「現在のブロック上面」Transform
    [HideInInspector] public Transform topBlockTransform;

    private TowerBlock currentBlock;
    private TowerBlock lastPlacedBlock;
    private int colorIndex = 0;
    private bool isSpawning = false;

    // 各ブロックの Y 座標（土台は 0）
    private float nextBlockY = 0f;

    public void StartSpawning()
    {
        isSpawning = true;
        colorIndex = 0;
        nextBlockY = 0f;
        lastPlacedBlock = null;
        currentBlock = null;

        // 土台ブロックを生成（動かない）
        SpawnFoundation();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (currentBlock != null)
        {
            currentBlock.isDropped = true;
        }
    }

    void Update()
    {
        if (!isSpawning) return;
        if (currentBlock == null || currentBlock.isDropped) return;

        // Space キー or 画面タップで Drop
        bool dropped = false;
        if (Input.GetKeyDown(KeyCode.Space)) dropped = true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) dropped = true;
        if (Input.GetMouseButtonDown(0)) dropped = true;

        if (dropped)
        {
            currentBlock.Drop();
        }
    }

    void SpawnFoundation()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Block_Foundation";
        go.transform.position = new Vector3(0f, nextBlockY, 0f);
        go.transform.localScale = new Vector3(blockWidth, blockHeight, blockDepth);

        var foundationColor = new Color(0.4f, 0.4f, 0.4f);
        var shader = ShaderUtil.GetLitShader();
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = foundationColor;
            go.GetComponent<MeshRenderer>().material = mat;
        }

        TowerBlock block = go.AddComponent<TowerBlock>();
        block.spawner = this;
        block.previousBlock = null;
        block.isDropped = true;  // 土台は最初から確定
        // Initialize を呼ぶことで Start() での色上書きを防ぐ
        block.Initialize(0f, 0f, foundationColor);

        lastPlacedBlock = block;
        topBlockTransform = go.transform;

        nextBlockY += blockHeight;
        colorIndex = 0;

        // 最初のブロックを生成
        SpawnNextBlock();
    }

    void SpawnNextBlock()
    {
        if (!isSpawning) return;

        float score = TowerGameManager.Instance != null ? TowerGameManager.Instance.Score : 0;
        float speed = baseSpeed + score * speedIncrement;

        Color color = blockColors[colorIndex % blockColors.Length];
        colorIndex++;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Block_" + score;
        // 前ブロックと同じ幅・奥行きから開始（スライスで変化する）
        float w = lastPlacedBlock != null ? lastPlacedBlock.transform.localScale.x : blockWidth;
        go.transform.localScale = new Vector3(w, blockHeight, blockDepth);
        go.transform.position = new Vector3(moveRange, nextBlockY, 0f);

        TowerBlock block = go.AddComponent<TowerBlock>();
        block.Initialize(speed, moveRange, color);
        block.spawner = this;
        block.previousBlock = lastPlacedBlock;

        currentBlock = block;
        topBlockTransform = go.transform;
    }

    /// <summary>
    /// TowerBlock.Slice() 完了後に呼ばれる。
    /// </summary>
    public void OnBlockPlaced(TowerBlock block)
    {
        lastPlacedBlock = block;
        topBlockTransform = block.transform;
        nextBlockY += blockHeight;

        TowerGameManager.Instance?.OnBlockStacked();

        SpawnNextBlock();
    }
}
