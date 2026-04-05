using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全カードのプール。レアリティ別絞り込みを提供する。
/// </summary>
[CreateAssetMenu(menuName = "StackTower/CardPool", fileName = "CardPool")]
public class CardPool : ScriptableObject
{
    public List<CardData> cards = new();

    public List<CardData> GetByRarity(CardRarity rarity) =>
        cards.FindAll(c => c != null && c.rarity == rarity);
}
