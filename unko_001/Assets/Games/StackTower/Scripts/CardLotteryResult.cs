/// <summary>
/// カード抽選の結果。card が null の場合は抽選失敗（プール不足など）。
/// isNew = false は重複当選（すでに所持済み）。
/// </summary>
public struct CardLotteryResult
{
    public CardData card;
    public bool isNew;
}
