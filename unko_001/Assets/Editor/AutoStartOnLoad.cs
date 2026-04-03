#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity Editor 起動時に自動実行される処理。
/// 現在: Unity MCP relay を自動起動する。
/// </summary>
[InitializeOnLoad]
public static class AutoStartOnLoad
{
    private static Process _relayProcess;

    static AutoStartOnLoad()
    {
        EditorApplication.delayCall += OnEditorReady;
    }

    static void OnEditorReady()
    {
        StartRelay();
    }

    static void StartRelay()
    {
        string relayPath = Path.GetFullPath(Path.Combine(
            Application.dataPath,
            "../Library/PackageCache/com.unity.ai.assistant@df09423d72a0/RelayApp~/relay_mac_arm64"
        ));

        if (!File.Exists(relayPath))
        {
            UnityEngine.Debug.LogWarning($"[AutoStart] relay が見つかりません: {relayPath}");
            return;
        }

        // 既存プロセスが動いていれば何もしない
        if (_relayProcess != null && !_relayProcess.HasExited)
        {
            UnityEngine.Debug.Log("[AutoStart] relay はすでに起動中です。");
            return;
        }

        Process.Start("chmod", $"+x \"{relayPath}\"");

        var startInfo = new ProcessStartInfo
        {
            FileName               = relayPath,
            Arguments              = "--mcp",
            UseShellExecute        = false,
            RedirectStandardOutput = false,
            RedirectStandardError  = false,
            CreateNoWindow         = true,
        };

        _relayProcess = Process.Start(startInfo);
        UnityEngine.Debug.Log("[AutoStart] Unity MCP relay を自動起動しました。");
    }
}
#endif
