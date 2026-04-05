using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Purchase dialog for removing ads.
/// Shows a "purchased" state if already bought.
/// </summary>
public class RemoveAdsDialog : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("State Views")]
    public GameObject unpurchasedView;  // Content shown before purchase
    public GameObject purchasedView;    // Content shown after purchase

    [Header("Buttons")]
    public Button purchaseButton;
    public Button restoreButton;    // Required for iOS review. Optional on Android.
    public Button closeButton;

    [Header("Text")]
    public TextMeshProUGUI statusText;  // Error or processing messages

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

    // ---- Button OnClick ----

    public void OnPurchaseButton()
    {
        SetStatus("Processing...");
        SetButtonsInteractable(false);
        IAPManager.Instance?.BuyRemoveAds();
    }

    public void OnRestoreButton()
    {
        SetStatus("Restoring...");
        SetButtonsInteractable(false);
        IAPManager.Instance?.RestorePurchases();
    }

    public void OnCloseButton() => Hide();

    // ---- Event handlers ----

    void HandlePurchaseSuccess()
    {
        Refresh();
        SetStatus("");
    }

    void HandleRestoreSuccess()
    {
        Refresh();
        SetStatus("Purchase restored successfully.");
    }

    void HandlePurchaseFailed(string message)
    {
        SetStatus(message);
        SetButtonsInteractable(true);
    }

    // ---- Internal ----

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
