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

    private const float PerfectThreshold = 0.1f;
    private const float GoodRatio = 0.35f;

    void Slice()
    {
        bool isX = moveAxis == MoveAxis.X;

        float prevCenter = isX ? previousBlock.transform.position.x : previousBlock.transform.position.z;
        float prevSize   = isX ? previousBlock.transform.localScale.x : previousBlock.transform.localScale.z;

        // Only for the first block above the foundation, widen the hit area by 1.7x
        if (previousBlock.previousBlock == null)
            prevSize *= 1.7f;
        float currCenter = isX ? transform.position.x : transform.position.z;
        float currSize   = isX ? transform.localScale.x : transform.localScale.z;

        float prevLeft  = prevCenter - prevSize / 2f;
        float prevRight = prevCenter + prevSize / 2f;
        float currLeft  = currCenter - currSize / 2f;
        float currRight = currCenter + currSize / 2f;

        float overlapLeft  = Mathf.Max(prevLeft,  currLeft);
        float overlapRight = Mathf.Min(prevRight, currRight);
        float overlapSize  = overlapRight - overlapLeft;

        if (overlapSize <= 0f)
        {
            SpawnDebris(currCenter, currSize, isX);
            gameObject.SetActive(false);
            TowerGameManager.Instance?.OnGameOver();
            return;
        }

        // Grace placement after continue: skip cutting, snap to previous center
        if (isFirstAfterContinue)
        {
            HapticManager.TriggerLight();
            if (isX)
                transform.position = new Vector3(prevCenter, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, prevCenter);
            spawner.OnBlockPlaced(this, PlacementQuality.Good);
            return;
        }

        float trimAmount = currSize - overlapSize;
        bool isFirst = previousBlock.previousBlock == null;
        PlacementQuality quality;

        if (!isFirst && trimAmount < PerfectThreshold)
            quality = PlacementQuality.Perfect;
        else if (trimAmount < currSize * GoodRatio)
            quality = PlacementQuality.Good;
        else
            quality = PlacementQuality.Bad;

        if (quality == PlacementQuality.Perfect)
        {
            HapticManager.TriggerLight();
            PerfectEffectManager.Instance?.PlayPerfect(transform.position);
            // Snap to the previous block's center
            if (isX)
                transform.position = new Vector3(prevCenter, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, prevCenter);
        }
        else
        {
            float leftDiff  = overlapLeft  - currLeft;
            float rightDiff = currRight - overlapRight;

            if (leftDiff > 0.01f)
                SpawnDebris(currLeft + leftDiff / 2f, leftDiff, isX);
            if (rightDiff > 0.01f)
                SpawnDebris(overlapRight + rightDiff / 2f, rightDiff, isX);

            float newCenter = (overlapLeft + overlapRight) / 2f;
            Vector3 scale = transform.localScale;
            if (isX) scale.x = overlapSize;
            else     scale.z = overlapSize;
            transform.localScale = scale;

            if (isX)
                transform.position = new Vector3(newCenter, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, newCenter);
        }

        spawner.OnBlockPlaced(this, quality);
    }

    void SpawnDebris(float center, float size, bool isXAxis)
    {
        if (size <= 0.01f) return;

        GameObject debris;
        if (spawner != null && spawner.meatPrefab != null)
        {
            debris = Object.Instantiate(spawner.meatPrefab);
        }
        else
        {
            debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var shader = ShaderUtil.GetLitShader();
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = blockColor * 0.7f;
                debris.GetComponent<MeshRenderer>().material = mat;
            }
        }

        debris.name = "Debris";
        spawner?.RegisterSpawnedObject(debris);

        if (isXAxis)
        {
            debris.transform.position  = new Vector3(center, transform.position.y, transform.position.z);
            debris.transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            debris.transform.position  = new Vector3(transform.position.x, transform.position.y, center);
            debris.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size);
        }

        // meatPrefab is expected to have a Collider; CreatePrimitive generates one automatically
        if (spawner != null && spawner.meatPrefab != null && debris.GetComponent<Collider>() == null)
            debris.AddComponent<BoxCollider>();
        Rigidbody rb = debris.AddComponent<Rigidbody>();
        rb.linearVelocity   = new Vector3(Random.Range(-1f, 1f), -1f, Random.Range(-1f, 1f));
        rb.angularVelocity  = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));

        Destroy(debris, 5f);
    }
}
