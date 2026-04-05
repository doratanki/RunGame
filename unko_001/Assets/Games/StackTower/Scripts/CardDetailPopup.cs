using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Full-screen card detail popup.
/// Unowned cards are shown as silhouette with "???".
/// </summary>
public class CardDetailPopup : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;
    public GameObject backdrop;   // Semi-transparent background. Tap to close.

    [Header("Card Display")]
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI unownedMessage;  // Shown when the card has not been obtained yet

    [Header("Unowned Color")]
    public Color unownedColor = new(0.15f, 0.15f, 0.15f, 1f);

    [Header("Rarity Colors")]
    public Color colorCommon    = Color.white;
    public Color colorRare      = new(0.3f, 0.7f, 1f);
    public Color colorEpic      = new(0.8f, 0.3f, 1f);
    public Color colorLegendary = new(1f, 0.8f, 0.1f);

    public void Show(CardData data)
    {
        if (panel != null) panel.SetActive(true);

        bool owned = CardOwnership.IsOwned(data.cardId);

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
            rarityText.text  = owned ? data.rarity.ToString() : "???";
            rarityText.color = owned ? RarityColor(data.rarity) : unownedColor;
        }

        if (unownedMessage != null)
            unownedMessage.gameObject.SetActive(!owned);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    // Assign this to the backdrop or close button OnClick
    public void OnCloseButton() => Hide();

    Color RarityColor(CardRarity rarity) => rarity switch
    {
        CardRarity.Common    => colorCommon,
        CardRarity.Rare      => colorRare,
        CardRarity.Epic      => colorEpic,
        CardRarity.Legendary => colorLegendary,
        _                    => Color.white,
    };
}
