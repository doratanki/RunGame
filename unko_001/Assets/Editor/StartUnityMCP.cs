#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity MCP relay を起動するエディターメニュー。
/// メニュー: Tools > Start Unity MCP
/// </summary>
public static class StartUnityMCP
{
    private static Process _relayProcess;

    [MenuItem("Tools/Start Unity MCP")]
    public static void StartRelay()
    {
        string relayPath = Path.Combine(
            Application.dataPath,
            "../Library/PackageCache/com.unity.ai.assistant@df09423d72a0/RelayApp~/relay_mac_arm64"
        );
        relayPath = Path.GetFullPath(relayPath);

        if (!File.Exists(relayPath))
        {
            UnityEngine.Debug.LogError($"[UnityMCP] relay が見つかりません: {relayPath}");
            return;
        }

        // 既存プロセスを終了
        if (_relayProcess != null && !_relayProcess.HasExited)
        {
            _relayProcess.Kill();
            _relayProcess = null;
            UnityEngine.Debug.Log("[UnityMCP] 既存の relay を停止しました。");
        }

        // 実行権限を付与
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
        UnityEngine.Debug.Log("[UnityMCP] relay を起動しました。Claude Code で /mcp reload unity を実行してください。");
    }
}
#endif
