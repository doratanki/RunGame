using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所持カード ID の永続管理。PlayerPrefs に JSON で保存する。
/// </summary>
public static class CardOwnership
{
    private const string PrefsKey = "OwnedCardIds";

    public static bool IsOwned(string cardId) => Load().Contains(cardId);

    public static void Add(string cardId)
    {
        var owned = Load();
        owned.Add(cardId);
        Save(owned);
    }

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
