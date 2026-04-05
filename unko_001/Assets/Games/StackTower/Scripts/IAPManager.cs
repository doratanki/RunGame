using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

/// <summary>
/// Singleton that manages the one-time "Remove Ads" purchase via Unity IAP.
///
/// Required package:
///   - com.unity.purchasing (Unity IAP 4.x)
///     Window > Package Manager > Unity Registry > In App Purchasing
///
/// Store setup:
///   - iOS  : Register an in-app purchase in App Store Connect and match the ProductId
///   - Android : Register an in-app item in Google Play Console and match the ProductId
/// </summary>
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance { get; private set; }

    // Use the same Product ID on both App Store and Google Play (must match the store entries)
    public const string ProductIdRemoveAds = "com.yourstudio.stacktower.removeads";

    private const string PrefsPurchasedKey = "RemoveAdsPurchased";

    IStoreController   _controller;
    IExtensionProvider _extensions;

    public bool IsRemoveAdsPurchased =>
        PlayerPrefs.GetInt(PrefsPurchasedKey, 0) == 1;

    // Events fired on purchase complete, restore complete, and purchase failure
    public event Action         OnPurchaseSuccess;
    public event Action         OnRestoreSuccess;
    public event Action<string> OnPurchaseFailedEvent;

    // ---- Lifecycle ----

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(ProductIdRemoveAds, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    // ---- Public API ----

    /// <summary>Purchase the Remove Ads product.</summary>
    public void BuyRemoveAds()
    {
        if (_controller == null)
        {
            OnPurchaseFailedEvent?.Invoke("Store is not initialized yet.");
            return;
        }
        _controller.InitiatePurchase(ProductIdRemoveAds);
    }

    /// <summary>
    /// Restore past purchases (required on iOS; Android restores automatically but provided for completeness).
    /// </summary>
    public void RestorePurchases()
    {
        if (_controller == null) return;

#if UNITY_IOS
        var apple = _extensions.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((result, error) =>
        {
            if (result)
            {
                Debug.Log("[IAP] Restore success.");
                OnRestoreSuccess?.Invoke();
            }
            else
            {
                Debug.LogWarning($"[IAP] Restore failed: {error}");
                OnPurchaseFailedEvent?.Invoke("Failed to restore purchases.");
            }
        });
#else
        // Android restores automatically on OnInitialized; just notify here
        if (IsRemoveAdsPurchased) OnRestoreSuccess?.Invoke();
        else OnPurchaseFailedEvent?.Invoke("No restorable purchases found.");
#endif
    }

    // ---- IStoreListener ----

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("[IAP] Initialized.");
        _controller = controller;
        _extensions = extensions;

        // Reflect purchase in PlayerPrefs if already owned (supports post-uninstall restore)
        var product = controller.products.WithID(ProductIdRemoveAds);
        if (product != null && product.hasReceipt)
            SetPurchased();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning($"[IAP] Init failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogWarning($"[IAP] Init failed: {error} - {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (args.purchasedProduct.definition.id == ProductIdRemoveAds)
        {
            SetPurchased();
            OnPurchaseSuccess?.Invoke();
            Debug.Log("[IAP] RemoveAds purchased.");
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        string msg = $"Purchase failed: {failureReason}";
        Debug.LogWarning($"[IAP] {msg}");
        OnPurchaseFailedEvent?.Invoke(msg);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        string msg = $"Purchase failed: {failureDescription.message}";
        Debug.LogWarning($"[IAP] {msg}");
        OnPurchaseFailedEvent?.Invoke(msg);
    }

    // ---- Internal ----

    void SetPurchased()
    {
        PlayerPrefs.SetInt(PrefsPurchasedKey, 1);
        PlayerPrefs.Save();
    }
}
