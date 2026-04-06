using UnityEngine;

/// <summary>
/// Manages a single block being stacked.
/// Calling Drop() stops movement and calculates the overlap with the previous block to Slice.
/// Supports both X-axis and Z-axis movement.
/// </summary>
public class TowerBlock : MonoBehaviour
{
    // Initialized by BlockSpawner
    [HideInInspector] public BlockSpawner spawner;
    [HideInInspector] public TowerBlock previousBlock; // Previous block (null = foundation)

    [HideInInspector] public bool isDropped = false;
    /// <summary>True for the first block placed after a continue. Skips cutting if the block isn't a complete miss.</summary>
    [HideInInspector] public bool isFirstAfterContinue = false;

    // Movement parameters (set by BlockSpawner)
    public float moveSpeed = 3f;
    private int moveDirection = -1;
    private float moveRange = 3f;
    [HideInInspector] public MoveAxis moveAxis = MoveAxis.X;

    // Block color (set by BlockSpawner)
    public Color blockColor = Color.white;
    private bool _colorInitialized = false;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
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

    public void Initialize(float speed, float range, Color color, MoveAxis axis = MoveAxis.X)
    {
        moveSpeed = speed;
        moveRange = range;
        blockColor = color;
        moveAxis = axis;
        _colorInitialized = true;
    }

    void Update()
    {
        if (isDropped) return;

        Vector3 dir = moveAxis == MoveAxis.X ? Vector3.right : Vector3.forward;
        transform.position += dir * moveSpeed * moveDirection * Time.deltaTime;

        float current = moveAxis == MoveAxis.X ? transform.position.x : transform.position.z;
        if (current > moveRange || current < -moveRange)
        {
            moveDirection *= -1;
            if (moveAxis == MoveAxis.X)
            {
                float clamped = Mathf.Clamp(transform.position.x, -moveRange, moveRange);
                transform.position = new Vector3(clamped, transform.position.y, transform.position.z);
            }
            else
            {
                float clamped = Mathf.Clamp(transform.position.z, -moveRange, moveRange);
                transform.position = new Vector3(transform.position.x, transform.position.y, clamped);
            }
        }
    }

    /// <summary>
    /// Called on tap/input. Stops the block and executes the slice.
    /// </summary>
    public void Drop()
    {
        if (isDropped) return;
        isDropped = true;

        if (previousBlock == null)
        {
            spawner.OnBlockPlaced(this);
            return;
        }

        Slice();
    }

    void Slice()
    {
        var cfg = TowerGameManager.Instance?.gameConfig;
        bool isX = moveAxis == MoveAxis.X;

        GetAxisValues(isX, cfg, out float prevCenter, out float prevSize, out float currCenter, out float currSize);
        CalculateOverlap(prevCenter, prevSize, currCenter, currSize,
            out float overlapLeft, out float overlapRight, out float overlapSize);

        if (overlapSize <= 0f)
        {
            HandleMiss(currCenter, currSize, isX);
            return;
        }

        if (isFirstAfterContinue)
        {
            SnapToCenter(prevCenter, isX);
            spawner.OnBlockPlaced(this, PlacementQuality.Good);
            return;
        }

        var quality = DetermineQuality(currSize - overlapSize, currSize, previousBlock.previousBlock == null, cfg);

        if (quality == PlacementQuality.Perfect)
        {
            HapticManager.TriggerLight();
            PerfectEffectManager.Instance?.PlayPerfect(transform.position);
            SnapToCenter(prevCenter, isX);
        }
        else
        {
            ApplyTrim(overlapLeft, overlapRight, overlapSize,
                currCenter - currSize / 2f, currCenter + currSize / 2f, isX);
        }

        spawner.OnBlockPlaced(this, quality);
    }

    void GetAxisValues(bool isX, GameConfig cfg,
        out float prevCenter, out float prevSize, out float currCenter, out float currSize)
    {
        float foundationHitMultiplier = cfg != null ? cfg.foundationHitMultiplier : 1.7f;

        prevCenter = isX ? previousBlock.transform.position.x : previousBlock.transform.position.z;
        prevSize   = isX ? previousBlock.transform.localScale.x : previousBlock.transform.localScale.z;

        // Only for the first block above the foundation, widen the hit area
        if (previousBlock.previousBlock == null)
            prevSize *= foundationHitMultiplier;

        currCenter = isX ? transform.position.x : transform.position.z;
        currSize   = isX ? transform.localScale.x : transform.localScale.z;
    }

    static void CalculateOverlap(float prevCenter, float prevSize, float currCenter, float currSize,
        out float overlapLeft, out float overlapRight, out float overlapSize)
    {
        float prevLeft  = prevCenter - prevSize / 2f;
        float prevRight = prevCenter + prevSize / 2f;
        float currLeft  = currCenter - currSize / 2f;
        float currRight = currCenter + currSize / 2f;

        overlapLeft  = Mathf.Max(prevLeft,  currLeft);
        overlapRight = Mathf.Min(prevRight, currRight);
        overlapSize  = overlapRight - overlapLeft;
    }

    static PlacementQuality DetermineQuality(float trimAmount, float currSize, bool isAboveFoundation, GameConfig cfg)
    {
        float perfectThreshold = cfg != null ? cfg.perfectThreshold : 0.1f;
        float goodRatio        = cfg != null ? cfg.goodRatio        : 0.35f;

        if (isAboveFoundation && trimAmount < perfectThreshold)
            return PlacementQuality.Perfect;
        if (trimAmount < currSize * goodRatio)
            return PlacementQuality.Good;
        return PlacementQuality.Bad;
    }

    void HandleMiss(float currCenter, float currSize, bool isX)
    {
        SpawnDebris(currCenter, currSize, isX);
        gameObject.SetActive(false);
        TowerGameManager.Instance?.OnGameOver();
    }

    void SnapToCenter(float prevCenter, bool isX)
    {
        HapticManager.TriggerLight();
        transform.position = isX
            ? new Vector3(prevCenter, transform.position.y, transform.position.z)
            : new Vector3(transform.position.x, transform.position.y, prevCenter);
    }

    void ApplyTrim(float overlapLeft, float overlapRight, float overlapSize,
        float currLeft, float currRight, bool isX)
    {
        float leftDiff  = overlapLeft  - currLeft;
        float rightDiff = currRight - overlapRight;

        if (leftDiff  > 0.01f) SpawnDebris(currLeft    + leftDiff  / 2f, leftDiff,  isX);
        if (rightDiff > 0.01f) SpawnDebris(overlapRight + rightDiff / 2f, rightDiff, isX);

        float newCenter = (overlapLeft + overlapRight) / 2f;
        Vector3 scale = transform.localScale;
        if (isX) scale.x = overlapSize;
        else     scale.z = overlapSize;
        transform.localScale = scale;

        transform.position = isX
            ? new Vector3(newCenter, transform.position.y, transform.position.z)
            : new Vector3(transform.position.x, transform.position.y, newCenter);
    }

    void SpawnDebris(float center, float size, bool isXAxis)
    {
        if (size <= 0.01f) return;
        if (spawner == null) return;

        // Fetch from pool instead of Instantiate/Destroy (reduces GC pressure)
        GameObject debris = spawner.debrisPool != null
            ? spawner.debrisPool.Get()
            : CreateFallbackDebris();

        if (debris == null) return;

        // Apply color to primitive debris (pooled prefab-based debris keeps its own material)
        if (spawner.meatPrefab == null)
        {
            var shader = ShaderUtil.GetLitShader();
            if (shader != null)
            {
                var mr = debris.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    var mat = new Material(shader);
                    mat.color = blockColor * 0.7f;
                    mr.material = mat;
                }
            }
        }

        // Position and scale
        if (isXAxis)
        {
            debris.transform.position   = new Vector3(center, transform.position.y, transform.position.z);
            debris.transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            debris.transform.position   = new Vector3(transform.position.x, transform.position.y, center);
            debris.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size);
        }

        // Ensure collider exists
        if (debris.GetComponent<Collider>() == null)
            debris.AddComponent<BoxCollider>();

        // Ensure Rigidbody exists and re-enable physics (pool returns it kinematic)
        if (!debris.TryGetComponent<Rigidbody>(out var rb))
            rb = debris.AddComponent<Rigidbody>();

        rb.isKinematic     = false;
        rb.linearVelocity  = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(-1f, 1f));
        rb.angularVelocity = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));

        // Return to pool after 5 s instead of Destroying
        var pool = spawner.debrisPool;
        if (pool != null)
            StartCoroutine(ReturnDebrisAfterDelay(debris, pool, 5f));
        else
            Destroy(debris, 5f);
    }

    static System.Collections.IEnumerator ReturnDebrisAfterDelay(GameObject debris, DebrisPool pool, float delay)
    {
        yield return new UnityEngine.WaitForSeconds(delay);
        pool.Return(debris);
    }

    static GameObject CreateFallbackDebris()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Debris";
        return go;
    }
}
