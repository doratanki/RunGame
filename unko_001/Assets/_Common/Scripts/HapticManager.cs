using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// Haptic feedback manager.
/// Call HapticManager.TriggerLight() to trigger a short vibration.
/// </summary>
public static class HapticManager
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _TriggerLightHaptic();
#endif

    public static void TriggerLight()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _TriggerLightHaptic();
#elif UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var plugin = new AndroidJavaClass("com.rungame.haptic.HapticPlugin");
            plugin.CallStatic("triggerLightHaptic", activity);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning($"[HapticManager] Android haptic failed: {e.Message}");
        }
#else
        // No-op in Editor
#endif
    }
}
