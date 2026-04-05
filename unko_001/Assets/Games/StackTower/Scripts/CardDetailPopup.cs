using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// カード拡大表示ポップアップ。
/// 未所持カードはシルエット＋"???"で表示する。
/// </summary>
public class CardDetailPopup : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;
    public GameObject backdrop;   // 半透明の背景。タップで閉じる用

    [Header("カード表示")]
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI unownedMessage;  // "まだ手に入れていないカードですわ" など

    [Header("未所持時のカラー")]
    public Color unownedColor = new(0.15f, 0.15f, 0.15f, 1f);

    [Header("レアリティ別カラー")]
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

    // backdrop や閉じるボタンの OnClick にアサインする
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
