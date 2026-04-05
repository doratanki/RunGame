using UnityEngine;

/// <summary>
/// カード1枚の定義。ScriptableObject で管理する。
/// </summary>
[CreateAssetMenu(menuName = "StackTower/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    [Tooltip("重複チェックに使う一意ID（例: card_001）")]
    public string cardId;
    public string cardName;
    public CardRarity rarity;
    public Sprite artwork;
}
