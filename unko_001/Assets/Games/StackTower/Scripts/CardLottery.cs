using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード抽選の純粋ロジック。MonoBehaviour 不要。
/// </summary>
public static class CardLottery
{
    /// <summary>
    /// ランクに基づいてカードを1枚抽選する。
    /// 重複当選は起こりうる（isNew = false で返す）。
    /// </summary>
    public static CardLotteryResult Draw(CardLotteryTable lotteryTable, CardPool pool, string rankLabel)
    {
        if (lotteryTable == null || pool == null)
            return default;

        var entry = lotteryTable.GetEntry(rankLabel);
        if (entry == null || entry.rates.Count == 0)
            return default;

        // レアリティを重み付き抽選で決定
        CardRarity rarity = PickRarity(entry.rates);

        // そのレアリティのカード一覧から1枚抽選
        var candidates = pool.GetByRarity(rarity);
        if (candidates.Count == 0)
            return default;

        CardData drawn = candidates[Random.Range(0, candidates.Count)];

        // 所持済み判定（重複当選はするが所持はしない）
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
