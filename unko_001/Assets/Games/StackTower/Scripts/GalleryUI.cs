using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gallery screen. Switches between a card tab and a rank tab.
/// Dynamically generates GalleryCardCell / GalleryRankCell in the ScrollView Content.
/// </summary>
public class GalleryUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Tab Buttons")]
    public Button cardTabButton;
    public Button rankTabButton;

    [Header("Tab Content")]
    public GameObject cardTabContent;
    public GameObject rankTabContent;

    [Header("Scroll")]
    public Transform cardGrid;   // ScrollView > Viewport > Content
    public Transform rankGrid;   // ScrollView > Viewport > Content

    [Header("Prefabs")]
    public GalleryCardCell cardCellPrefab;
    public GalleryRankCell rankCellPrefab;

    [Header("Data")]
    public CardPool  cardPool;
    public RankTable rankTable;

    [Header("Owned Count Text (optional)")]
    public TextMeshProUGUI ownedCountText;

    [Header("Card Detail Popup")]
    public CardDetailPopup cardDetailPopup;

    bool _built;

    public void Show()
    {
        if (panel != null) panel.SetActive(true);

        if (!_built)
        {
            BuildCardGrid();
            BuildRankGrid();
            _built = true;
        }
        else
        {
            // Ownership may have changed, so refresh
            RebuildCardGrid();
        }

        ShowCardTab();
        UpdateOwnedCount();
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    // ---- Tab switching ----

    public void ShowCardTab()
    {
        if (cardTabContent != null) cardTabContent.SetActive(true);
        if (rankTabContent != null) rankTabContent.SetActive(false);
    }

    public void ShowRankTab()
    {
        if (cardTabContent != null) cardTabContent.SetActive(false);
        if (rankTabContent != null) rankTabContent.SetActive(true);
    }

    // ---- Grid building ----

    void BuildCardGrid()
    {
        if (cardPool == null || cardCellPrefab == null || cardGrid == null) return;

        foreach (var data in cardPool.cards)
        {
            if (data == null) continue;
            var cell = Instantiate(cardCellPrefab, cardGrid);
            cell.Setup(data, OnCardTapped);
        }
    }

    void RebuildCardGrid()
    {
        if (cardGrid == null) return;

        // Update existing cells without recreating them
        int i = 0;
        foreach (Transform child in cardGrid)
        {
            var cell = child.GetComponent<GalleryCardCell>();
            if (cell != null && cardPool != null && i < cardPool.cards.Count)
                cell.Setup(cardPool.cards[i++], OnCardTapped);
        }
    }

    void BuildRankGrid()
    {
        if (rankTable == null || rankCellPrefab == null || rankGrid == null) return;

        var entries = rankTable.entries;
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry == null) continue;

            var cell = Instantiate(rankCellPrefab, rankGrid);
            cell.Setup(entry);

            int? nextMin = (i + 1 < entries.Count) ? entries[i + 1].minScore : (int?)null;
            cell.SetScoreRange(entry.minScore, nextMin);
        }
    }

    void OnCardTapped(CardData data) => cardDetailPopup?.Show(data);

    void UpdateOwnedCount()
    {
        if (ownedCountText == null || cardPool == null) return;

        int total = cardPool.cards.Count;
        int owned = 0;
        foreach (var data in cardPool.cards)
            if (data != null && CardOwnership.IsOwned(data.cardId)) owned++;

        ownedCountText.text = $"{owned} / {total}";
    }
}
