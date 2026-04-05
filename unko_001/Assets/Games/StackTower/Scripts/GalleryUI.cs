using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ギャラリー画面。カードタブとランクタブを切り替えて表示する。
/// ScrollView の Content に GalleryCardCell / GalleryRankCell を動的生成する。
/// </summary>
public class GalleryUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("タブボタン")]
    public Button cardTabButton;
    public Button rankTabButton;

    [Header("タブ内容")]
    public GameObject cardTabContent;
    public GameObject rankTabContent;

    [Header("スクロール")]
    public Transform cardGrid;   // ScrollView > Viewport > Content
    public Transform rankGrid;   // ScrollView > Viewport > Content

    [Header("プレハブ")]
    public GalleryCardCell cardCellPrefab;
    public GalleryRankCell rankCellPrefab;

    [Header("データ")]
    public CardPool  cardPool;
    public RankTable rankTable;

    [Header("所持数テキスト（省略可）")]
    public TextMeshProUGUI ownedCountText;

    [Header("カード詳細ポップアップ")]
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
            // 所持状況が変わっている可能性があるので再描画
            RebuildCardGrid();
        }

        ShowCardTab();
        UpdateOwnedCount();
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    // ---- タブ切り替え ----

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

    // ---- グリッド構築 ----

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

        // 既存セルに再適用（再生成せず状態だけ更新）
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
