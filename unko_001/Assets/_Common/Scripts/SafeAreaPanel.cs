using UnityEngine;

/// <summary>
/// アタッチした RectTransform をデバイスのセーフエリアに合わせて調整する。
/// Canvas 直下に空の GameObject を作り、全 UI をその子にしてこのスクリプトをアタッチする。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform _rect;
    private Rect _lastSafeArea;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (_lastSafeArea != Screen.safeArea)
            Apply();
#endif
    }

    void Apply()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        _rect.anchorMin = anchorMin;
        _rect.anchorMax = anchorMax;
        _rect.offsetMin = Vector2.zero;
        _rect.offsetMax = Vector2.zero;
    }
}
