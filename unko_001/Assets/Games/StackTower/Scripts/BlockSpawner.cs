using System.Collections.Generic;
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

    [Header("土台モデル（設定するとCubeの代わりに使用）")]
    public GameObject bbqTablePrefab;

    [Header("肉モデル（設定するとCubeの代わりに使用）")]
    public GameObject meatPrefab;

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
    private bool _useInitialSizeOnce = false;

    private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

    // 各ブロックの Y 座標（土台は 0）
    private float nextBlockY = 0f;

    // 次に生成するブロックの移動軸（X→Z→X→... と交互）
    private MoveAxis _nextAxis = MoveAxis.X;

    public void StartSpawning()
    {
        isSpawning = true;
        colorIndex = 0;
        nextBlockY = 0f;
        lastPlacedBlock = null;
        currentBlock = null;
        _nextAxis = MoveAxis.X;

        // 土台ブロックを生成（動かない）
        SpawnFoundation();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (currentBlock != null)
            currentBlock.isDropped = true;
    }

    /// <summary>
    /// コンティニュー時に呼ぶ。現在の塔の高さからブロックを初期サイズで再開する。
    /// </summary>
    public void ContinueSpawning()
    {
        isSpawning = true;
        _useInitialSizeOnce = true;
        SpawnNextBlock();
    }

    public void RegisterSpawnedObject(GameObject go)
    {
        _spawnedObjects.Add(go);
    }

    public void ClearBlocks()
    {
        isSpawning = false;
        _useInitialSizeOnce = false;

        foreach (var go in _spawnedObjects)
        {
            if (go != null)
            {
                go.SetActive(false);   // 即座に非表示にしてから破棄
                Object.Destroy(go);
            }
        }
        _spawnedObjects.Clear();

        lastPlacedBlock = null;
        currentBlock = null;
        topBlockTransform = null;
        nextBlockY = 0f;
        colorIndex = 0;
        _nextAxis = MoveAxis.X;
    }

    void Update()
    {
        if (!isSpawning) return;
        if (currentBlock == null || currentBlock.isDropped) return;

        // 広告表示中は入力を無視
        if (AdsManager.Instance != null && AdsManager.Instance.IsAdShowing) return;

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
        var foundationColor = new Color(0.4f, 0.4f, 0.4f);
        GameObject go;

        if (bbqTablePrefab != null)
        {
            go = Object.Instantiate(bbqTablePrefab);
            go.name = "Block_Foundation";
            go.transform.position = new Vector3(0f, nextBlockY - 2f, 0f);
            // x/z を blockWidth/blockDepth に合わせることで、次ブロックが正しいサイズを引き継ぐ
            go.transform.localScale = new Vector3(blockWidth, 1f, blockDepth);

            // BBQTable の実際の上端から次ブロックのY座標を決める
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                foreach (var r in renderers)
                    bounds.Encapsulate(r.bounds);
                nextBlockY = bounds.max.y;
            }
            else
            {
                nextBlockY += blockHeight;
            }
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Block_Foundation";
            go.transform.position = new Vector3(0f, nextBlockY, 0f);
            go.transform.localScale = new Vector3(blockWidth, blockHeight, blockDepth);

            var shader = ShaderUtil.GetLitShader();
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = foundationColor;
                go.GetComponent<MeshRenderer>().material = mat;
            }
            nextBlockY += blockHeight;
        }

        _spawnedObjects.Add(go);
        TowerBlock block = go.AddComponent<TowerBlock>();
        block.spawner = this;
        block.previousBlock = null;
        block.isDropped = true;
        block.Initialize(0f, 0f, foundationColor);

        lastPlacedBlock = block;
        topBlockTransform = go.transform;
        colorIndex = 0;

        SpawnNextBlock();
    }

    void SpawnNextBlock()
    {
        if (!isSpawning) return;

        float score = TowerGameManager.Instance != null ? TowerGameManager.Instance.Score : 0;
        float speed = baseSpeed + score * speedIncrement;

        MoveAxis axis = _nextAxis;
        _nextAxis = axis == MoveAxis.X ? MoveAxis.Z : MoveAxis.X;

        // 前ブロックの幅・奥行を引き継ぐ（コンティニュー時は初期サイズに戻す）
        float w, d;
        if (_useInitialSizeOnce)
        {
            w = blockWidth;
            d = blockDepth;
            _useInitialSizeOnce = false;
        }
        else
        {
            w = lastPlacedBlock != null ? lastPlacedBlock.transform.localScale.x : blockWidth;
            d = lastPlacedBlock != null ? lastPlacedBlock.transform.localScale.z : blockDepth;
        }

        // 前ブロックの中心位置（固定軸は前ブロックに揃える）
        float prevX = lastPlacedBlock != null ? lastPlacedBlock.transform.position.x : 0f;
        float prevZ = lastPlacedBlock != null ? lastPlacedBlock.transform.position.z : 0f;

        // スタート位置：移動軸は +moveRange、固定軸は前ブロックの中心
        Vector3 spawnPos = axis == MoveAxis.X
            ? new Vector3(moveRange, nextBlockY, prevZ)
            : new Vector3(prevX, nextBlockY, moveRange);

        GameObject go;
        if (meatPrefab != null)
        {
            go = Object.Instantiate(meatPrefab);
            go.name = "Block_" + score;
            go.transform.localScale = new Vector3(w, blockHeight, d);
            go.transform.position = spawnPos;
        }
        else
        {
            Color color = blockColors[colorIndex % blockColors.Length];
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Block_" + score;
            go.transform.localScale = new Vector3(w, blockHeight, d);
            go.transform.position = spawnPos;

            var shader = ShaderUtil.GetLitShader();
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = color;
                go.GetComponent<MeshRenderer>().material = mat;
            }
        }

        colorIndex++;
        _spawnedObjects.Add(go);

        TowerBlock block = go.AddComponent<TowerBlock>();
        block.Initialize(speed, moveRange, Color.white, axis);
        block.spawner = this;
        block.previousBlock = lastPlacedBlock;

        currentBlock = block;
        topBlockTransform = go.transform;
    }

    /// <summary>
    /// TowerBlock.Slice() 完了後に呼ばれる。
    /// </summary>
    public void OnBlockPlaced(TowerBlock block, PlacementQuality quality = PlacementQuality.Good)
    {
        lastPlacedBlock = block;
        topBlockTransform = block.transform;
        nextBlockY += blockHeight;

        TowerGameManager.Instance?.OnBlockStacked(quality);

        SpawnNextBlock();
    }
}
