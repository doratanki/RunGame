/// <summary>
/// スコア → ランク変換の純粋ロジック。MonoBehaviour 不要。
/// </summary>
public static class RankCalculator
{
    /// <summary>
    /// score に対応する RankEntry を返す。
    /// entries が昇順前提。該当なければ先頭（最低ランク）を返す。
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
    /// 現在のスコアの次のランクを返す。最高ランクなら null。
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
