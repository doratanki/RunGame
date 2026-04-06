using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pure card lottery logic. No MonoBehaviour required.
/// </summary>
public static class CardLottery
{
    /// <summary>
    /// Draws one card based on the given rank.
    /// Duplicate draws can occur (returned with isNew = false).
    /// </summary>
    public static CardLotteryResult Draw(CardLotteryTable lotteryTable, CardPool pool, string rankLabel)
    {
        if (lotteryTable == null)
        {
            Debug.LogError("[CardLottery] lotteryTable is null. Check Inspector assignment.");
            return default;
        }
        if (pool == null)
        {
            Debug.LogError("[CardLottery] cardPool is null. Check Inspector assignment.");
            return default;
        }
        if (string.IsNullOrEmpty(rankLabel))
        {
            Debug.LogError("[CardLottery] rankLabel is null or empty.");
            return default;
        }

        var entry = lotteryTable.GetEntry(rankLabel);
        if (entry == null)
        {
            Debug.LogError($"[CardLottery] No entry found for rank '{rankLabel}' in lotteryTable.");
            return default;
        }
        if (entry.rates.Count == 0)
        {
            Debug.LogError($"[CardLottery] Rank '{rankLabel}' has no rarity rates configured.");
            return default;
        }

        // Determine rarity via weighted random
        CardRarity rarity = PickRarity(entry.rates);

        // Draw one card from those matching the rarity
        var candidates = pool.GetByRarity(rarity);
        if (candidates.Count == 0)
        {
            Debug.LogError($"[CardLottery] No cards of rarity '{rarity}' found in pool.");
            return default;
        }

        CardData drawn = candidates[Random.Range(0, candidates.Count)];

        // Check ownership (duplicates are drawn but not re-added)
        bool isNew = !CardOwnership.IsOwned(drawn.cardId);
        if (isNew)
            CardOwnership.Add(drawn.cardId);

        return new CardLotteryResult { card = drawn, isNew = isNew };
    }

    static CardRarity PickRarity(List<RarityRate> rates)
    {
        float total = 0f;
        foreach (var r in rates) total += r.weight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var r in rates)
        {
            cumulative += r.weight;
            if (roll < cumulative) return r.rarity;
        }
        return rates[rates.Count - 1].rarity;
    }
}
