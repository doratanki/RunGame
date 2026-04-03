using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// ハプティクス（バイブ）管理クラス。
/// HapticManager.TriggerLight() を呼ぶだけで最短バイブが発生する。
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
        // Editor では何もしない
#endif
    }
}
