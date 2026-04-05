using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

/// <summary>
/// Unity IAP による買い切り「広告削除」購入を管理するシングルトン。
///
/// 必要パッケージ:
///   - com.unity.purchasing (Unity IAP 4.x)
///     Window > Package Manager > Unity Registry > In App Purchasing
///
/// ストア設定:
///   - iOS  : App Store Connect でアプリ内課金を登録し ProductId を合わせる
///   - Android : Google Play Console でアプリ内アイテムを登録し ProductId を合わせる
/// </summary>
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance { get; private set; }

    // ▼ App Store / Google Play 双方で同じ Product ID を使う（ストア側と一致させること）
    public const string ProductIdRemoveAds = "com.yourstudio.stacktower.removeads";

    private const string PrefsPurchasedKey = "RemoveAdsPurchased";

    IStoreController   _controller;
    IExtensionProvider _extensions;

    public bool IsRemoveAdsPurchased =>
        PlayerPrefs.GetInt(PrefsPurchasedKey, 0) == 1;

    // 購入完了・復元完了を通知するイベント
    public event Action         OnPurchaseSuccess;
    public event Action         OnRestoreSuccess;
    public event Action<string> OnPurchaseFailedEvent;

    // ---- ライフサイクル ----

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

    // ---- 外部 API ----

    /// <summary>広告削除を購入する。</summary>
    public void BuyRemoveAds()
    {
        if (_controller == null)
        {
            OnPurchaseFailedEvent?.Invoke("ストアの初期化が完了していません。");
            return;
        }
        _controller.InitiatePurchase(ProductIdRemoveAds);
    }

    /// <summary>
    /// 購入履歴を復元する（iOS 必須、Android は自動復元されるが念のため用意）。
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
                OnPurchaseFailedEvent?.Invoke("購入の復元に失敗しました。");
            }
        });
#else
        // Android は OnInitialized 時に自動復元されるためここでは通知のみ
        if (IsRemoveAdsPurchased) OnRestoreSuccess?.Invoke();
        else OnPurchaseFailedEvent?.Invoke("復元できる購入履歴が見つかりません。");
#endif
    }

    // ---- IStoreListener ----

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("[IAP] Initialized.");
        _controller = controller;
        _extensions = extensions;

        // 購入済みなら PlayerPrefs に反映（アンインストール後の復元対応）
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
        string msg = $"購入失敗: {failureReason}";
        Debug.LogWarning($"[IAP] {msg}");
        OnPurchaseFailedEvent?.Invoke(msg);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        string msg = $"購入失敗: {failureDescription.message}";
        Debug.LogWarning($"[IAP] {msg}");
        OnPurchaseFailedEvent?.Invoke(msg);
    }

    // ---- 内部 ----

    void SetPurchased()
    {
        PlayerPrefs.SetInt(PrefsPurchasedKey, 1);
        PlayerPrefs.Save();
    }
}
