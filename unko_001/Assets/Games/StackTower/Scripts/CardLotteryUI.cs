using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カード抽選結果を表示するパネル。
/// </summary>
public class CardLotteryUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("カード表示")]
    public Image             cardArtwork;
    public TextMeshProUGUI   cardNameText;
    public TextMeshProUGUI   rarityText;
    public TextMeshProUGUI   newLabel;       // "NEW!" 表示。重複時は非表示

    [Header("レアリティ別カラー")]
    public Color colorCommon    = Color.white;
    public Color colorRare      = new(0.3f, 0.7f, 1f);
    public Color colorEpic      = new(0.8f, 0.3f, 1f);
    public Color colorLegendary = new(1f, 0.8f, 0.1f);

    public void Show(CardLotteryResult result)
    {
        if (panel != null) panel.SetActive(true);
        if (result.card == null) return;

        if (cardArtwork != null)
        {
            cardArtwork.sprite  = result.card.artwork;
            cardArtwork.enabled = result.card.artwork != null;
        }

        if (cardNameText != null)
            cardNameText.text = result.card.cardName;

        if (rarityText != null)
        {
            rarityText.text  = result.card.rarity.ToString();
            rarityText.color = RarityColor(result.card.rarity);
        }

        if (newLabel != null)
            newLabel.gameObject.SetActive(result.isNew);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
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
