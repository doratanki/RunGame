using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 広告削除購入ダイアログ。
/// 購入済みの場合は購入ボタンを非表示にして「購入済み」を表示する。
/// </summary>
public class RemoveAdsDialog : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("状態別表示")]
    public GameObject unpurchasedView;  // 未購入時に表示するコンテンツ
    public GameObject purchasedView;    // 購入済み時に表示するコンテンツ

    [Header("ボタン")]
    public Button purchaseButton;
    public Button restoreButton;    // iOS 審査要件。Android では省略可
    public Button closeButton;

    [Header("テキスト")]
    public TextMeshProUGUI statusText;  // エラーや処理中メッセージ

    void OnEnable()
    {
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnPurchaseSuccess += HandlePurchaseSuccess;
            IAPManager.Instance.OnRestoreSuccess  += HandleRestoreSuccess;
            IAPManager.Instance.OnPurchaseFailedEvent  += HandlePurchaseFailed;
        }
    }

    void OnDisable()
    {
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnPurchaseSuccess -= HandlePurchaseSuccess;
            IAPManager.Instance.OnRestoreSuccess  -= HandleRestoreSuccess;
            IAPManager.Instance.OnPurchaseFailedEvent  -= HandlePurchaseFailed;
        }
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    // ---- ボタン OnClick ----

    public void OnPurchaseButton()
    {
        SetStatus("処理中...");
        SetButtonsInteractable(false);
        IAPManager.Instance?.BuyRemoveAds();
    }

    public void OnRestoreButton()
    {
        SetStatus("復元中...");
        SetButtonsInteractable(false);
        IAPManager.Instance?.RestorePurchases();
    }

    public void OnCloseButton() => Hide();

    // ---- イベントハンドラ ----

    void HandlePurchaseSuccess()
    {
        Refresh();
        SetStatus("");
    }

    void HandleRestoreSuccess()
    {
        Refresh();
        SetStatus("購入履歴を復元しました。");
    }

    void HandlePurchaseFailed(string message)
    {
        SetStatus(message);
        SetButtonsInteractable(true);
    }

    // ---- 内部 ----

    void Refresh()
    {
        bool purchased = IAPManager.Instance != null && IAPManager.Instance.IsRemoveAdsPurchased;

        if (unpurchasedView != null) unpurchasedView.SetActive(!purchased);
        if (purchasedView   != null) purchasedView.SetActive(purchased);

        SetButtonsInteractable(true);
        SetStatus("");
    }

    void SetButtonsInteractable(bool value)
    {
        if (purchaseButton != null) purchaseButton.interactable = value;
        if (restoreButton  != null) restoreButton.interactable  = value;
    }

    void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }
}
