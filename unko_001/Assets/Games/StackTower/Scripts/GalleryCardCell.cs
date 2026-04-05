using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カードギャラリーの1セル。
/// 未所持は暗転・名前を "???" で隠す。
/// タップ時に onTapped(CardData) を呼ぶ。
/// </summary>
public class GalleryCardCell : MonoBehaviour
{
    [Header("UI")]
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI rarityText;
    public GameObject      newBadge;     // 最近取得したカードにバッジ表示（省略可）
    public Button          button;       // セル全体に被せる Button

    [Header("未所持時のカラー")]
    public Color unownedColor = new(0.2f, 0.2f, 0.2f, 1f);

    [Header("レアリティ別カラー")]
    public Color colorCommon    = Color.white;
    public Color colorRare      = new(0.3f, 0.7f, 1f);
    public Color colorEpic      = new(0.8f, 0.3f, 1f);
    public Color colorLegendary = new(1f, 0.8f, 0.1f);

    CardData _data;

    public void Setup(CardData data, Action<CardData> onTapped = null)
    {
        _data = data;
        bool owned = CardOwnership.IsOwned(data.cardId);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onTapped?.Invoke(_data));
        }

        if (artwork != null)
        {
            artwork.sprite  = data.artwork;
            artwork.color   = owned ? Color.white : unownedColor;
            artwork.enabled = data.artwork != null;
        }

        if (cardNameText != null)
            cardNameText.text = owned ? data.cardName : "???";

        if (rarityText != null)
        {
            rarityText.text  = data.rarity.ToString();
            rarityText.color = owned ? RarityColor(data.rarity) : unownedColor;
        }

        if (newBadge != null)
            newBadge.SetActive(false); // 将来的に「新着」マーク拡張用
    }

    Color RarityColor(CardRarity rarity) => rarity switch
    {
        CardRarity.Common    => colorCommon,
        CardRarity.Rare      => colorRare,
        CardRarity.Epic      => colorEpic,
        CardRarity.Legendary => colorLegendary,
        _                    => Color.white,
    };
}
