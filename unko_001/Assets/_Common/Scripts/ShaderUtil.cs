using UnityEngine;

/// <summary>
/// Common utility that caches and returns the URP / Standard shader.
/// Shared across games.
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
