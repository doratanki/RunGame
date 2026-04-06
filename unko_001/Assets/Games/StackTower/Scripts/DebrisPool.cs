using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pool for debris GameObjects spawned when blocks are trimmed.
/// Attach to the same GameObject as BlockSpawner.
/// Reduces GC pressure from frequent Instantiate/Destroy calls.
/// </summary>
public class DebrisPool : MonoBehaviour
{
    [Tooltip("Number of debris objects pre-allocated at startup.")]
    public int initialPoolSize = 20;

    private readonly Queue<GameObject> _available = new();
    private readonly List<GameObject>  _all       = new();
    private GameObject _prefab;

    public void Initialize(GameObject prefab)
    {
        _prefab = prefab;
        for (int i = 0; i < initialPoolSize; i++)
            _available.Enqueue(CreateNew());
    }

    /// <summary>Returns a debris object from the pool (or creates one if the pool is empty).</summary>
    public GameObject Get()
    {
        GameObject obj = _available.Count > 0 ? _available.Dequeue() : CreateNew();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>Returns a debris object to the pool after use.</summary>
    public void Return(GameObject obj)
    {
        if (obj == null) return;

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic     = true;
        }

        obj.SetActive(false);
        _available.Enqueue(obj);
    }

    /// <summary>Returns all active debris to the pool immediately (called by ClearBlocks).</summary>
    public void ReturnAll()
    {
        foreach (var obj in _all)
        {
            if (obj != null && obj.activeSelf)
                Return(obj);
        }
    }

    GameObject CreateNew()
    {
        GameObject obj;
        if (_prefab != null)
        {
            obj = Instantiate(_prefab);
        }
        else
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // Remove the auto-added MeshCollider; BoxCollider added on Get() if needed
            var meshCol = obj.GetComponent<MeshCollider>();
            if (meshCol != null) Destroy(meshCol);
        }

        obj.name = "Debris(Pooled)";
        obj.SetActive(false);
        _all.Add(obj);
        return obj;
    }

    void OnDestroy()
    {
        foreach (var obj in _all)
        {
            if (obj != null)
                Destroy(obj);
        }
        _all.Clear();
        _available.Clear();
    }
}
