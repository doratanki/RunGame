using UnityEngine;

/// <summary>
/// URP / Standard シェーダーをキャッシュして返す共通ユーティリティ。
/// ゲームをまたいで利用する。
/// </summary>
public static class ShaderUtil
{
    private static Shader _cached;

    public static Shader GetLitShader()
    {
        if (_cached == null)
            _cached = Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Standard");
        return _cached;
    }
}
