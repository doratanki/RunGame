/// <summary>
/// Generic singleton base class for MonoBehaviour.
/// Subclasses that need extra Awake logic should override Awake,
/// call base.Awake() first, then guard with: if (Instance != this) return;
/// </summary>
public abstract class Singleton<T> : UnityEngine.MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = this as T;
        else
        {
            UnityEngine.Debug.LogWarning(
                $"[Singleton] Duplicate instance of {typeof(T).Name} detected. Destroying the new one.");
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
