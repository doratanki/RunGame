using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所持カード ID の永続管理。PlayerPrefs に JSON で保存する。
/// 読み込み結果をメモリにキャッシュし、IsOwned() のたびに JSON をパースしない。
/// </summary>
public static class CardOwnership
{
    private const string PrefsKey = "OwnedCardIds";

    private static HashSet<string> _cache;

    static HashSet<string> Cache => _cache ??= Load();

    public static bool IsOwned(string cardId) => Cache.Contains(cardId);

    public static void Add(string cardId)
    {
        Cache.Add(cardId);
        Save(Cache);
    }

    /// <summary>
    /// Clears the in-memory cache, forcing a reload from PlayerPrefs on next access.
    /// Call this if PlayerPrefs may have been modified externally.
    /// </summary>
    public static void InvalidateCache() => _cache = null;

    static HashSet<string> Load()
    {
        string json = PlayerPrefs.GetString(PrefsKey, "{\"ids\":[]}");
        var wrapper = JsonUtility.FromJson<IdList>(json);
        return new HashSet<string>(wrapper.ids ?? new List<string>());
    }

    static void Save(HashSet<string> owned)
    {
        var wrapper = new IdList { ids = new List<string>(owned) };
        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    [Serializable]
    class IdList { public List<string> ids = new(); }
}
