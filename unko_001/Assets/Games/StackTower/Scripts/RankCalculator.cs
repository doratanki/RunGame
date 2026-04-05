/// <summary>
/// Pure logic for score → rank conversion. No MonoBehaviour required.
/// </summary>
public static class RankCalculator
{
    /// <summary>
    /// Returns the RankEntry corresponding to the given score.
    /// Assumes entries are in ascending order. Returns the first entry (lowest rank) if none match.
    /// </summary>
    public static RankEntry GetRank(RankTable table, int score)
    {
        if (table == null || table.entries == null || table.entries.Count == 0)
            return null;

        RankEntry result = table.entries[0];
        foreach (var entry in table.entries)
        {
            if (score >= entry.minScore)
                result = entry;
            else
                break;
        }
        return result;
    }

    /// <summary>
    /// Returns the next rank above the current score. Returns null if already at the highest rank.
    /// </summary>
    public static RankEntry GetNextRank(RankTable table, int score)
    {
        if (table == null || table.entries == null) return null;

        for (int i = 0; i < table.entries.Count - 1; i++)
        {
            if (score < table.entries[i + 1].minScore)
                return table.entries[i + 1];
        }
        return null;
    }
}
