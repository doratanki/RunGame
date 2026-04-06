using UnityEngine;

/// <summary>
/// Adjusts the attached RectTransform to match the device's safe area.
/// Create an empty GameObject directly under the Canvas, place all UI as its children,
/// and attach this script to it.
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
        if (_lastSafeArea != Screen.safeArea)
            Apply();
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
