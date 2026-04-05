using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages block spawning, movement, and input handling.
/// Call StartSpawning() from TowerGameManager to start the game.
/// </summary>
public class BlockSpawner : MonoBehaviour
{
    [Header("Block Settings")]
    public float blockWidth  = 3f;
    public float blockHeight = 0.4f;
    public float blockDepth  = 3f;
    public float moveRange   = 3f;  // X-axis sweep range (±)

    [Header("Difficulty")]
    public float baseSpeed      = 2.5f;
    public float speedIncrement = 0.1f;  // Speed increase per score point

    [Header("Foundation Model (uses Cube if not set)")]
    public GameObject bbqTablePrefab;

    [Header("Block Model (uses Cube if not set)")]
    public GameObject meatPrefab;

    [Header("Color Palette (set in Inspector)")]
    public Color[] blockColors = new Color[]
    {
        new Color(0.95f, 0.35f, 0.35f),
        new Color(0.35f, 0.75f, 0.95f),
        new Color(0.35f, 0.95f, 0.55f),
        new Color(0.95f, 0.85f, 0.25f),
        new Color(0.85f, 0.45f, 0.95f),
    };

    // Transform of the current block's top face, used by CameraFollow
    [HideInInspector] public Transform topBlockTransform;

    private TowerBlock currentBlock;
    private TowerBlock lastPlacedBlock;
    private int colorIndex = 0;
    private bool isSpawning = false;
    private bool _useInitialSizeOnce = false;

    private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

    // Y position for the next block (foundation is at 0)
    private float nextBlockY = 0f;

    // Movement axis for the next block (alternates X → Z → X → ...)
    private MoveAxis _nextAxis = MoveAxis.X;

    public void StartSpawning()
    {
        isSpawning = true;
        colorIndex = 0;
        nextBlockY = 0f;
        lastPlacedBlock = null;
        currentBlock = null;
        _nextAxis = MoveAxis.X;

        // Spawn the foundation block (stationary)
        SpawnFoundation();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (currentBlock != null)
            currentBlock.isDropped = true;
    }

    /// <summary>
    /// Call on continue. Resumes spawning from the current tower height with the initial block size.
    /// </summary>
    public void ContinueSpawning()
    {
        isSpawning = true;
        _useInitialSizeOnce = true;
        SpawnNextBlock();
        // Mark the first block after continue — it gets a grace placement (no cut on non-miss)
        if (currentBlock != null)
            currentBlock.isFirstAfterContinue = true;
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
                go.SetActive(false);   // Hide immediately before destroying
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

        // Block input while an ad is showing
        if (AdsManager.Instance != null && AdsManager.Instance.IsAdShowing) return;

        // Drop on Space, screen tap, or mouse click
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
            // Match x/z to blockWidth/blockDepth so the next block inherits the correct size
            go.transform.localScale = new Vector3(blockWidth, 1f, blockDepth);

            // Determine nextBlockY from the actual top of the BBQTable bounds
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

        // Inherit width/depth from the previous block (reset to initial size on continue)
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

        // Fixed axis aligns to the previous block's center
        float prevX = lastPlacedBlock != null ? lastPlacedBlock.transform.position.x : 0f;
        float prevZ = lastPlacedBlock != null ? lastPlacedBlock.transform.position.z : 0f;

        // Start position: moving axis starts at +moveRange, fixed axis aligns to previous block
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
    /// Called after TowerBlock.Slice() completes.
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
