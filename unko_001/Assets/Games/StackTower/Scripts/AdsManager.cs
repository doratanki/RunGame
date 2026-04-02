using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Unity Ads を管理するシングルトン。
/// TowerGameManager と同じ GameObject にアタッチする。
///
/// 使い方:
///   1. Inspector で gameIdIos / gameIdAndroid を設定
///   2. TowerGameManager.OnGameOver() の末尾で AdsManager.Instance?.ShowInterstitial() を呼ぶ
/// </summary>
public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance { get; private set; }

    [Header("Game ID（Unity Dashboard で確認）")]
    public string gameIdIos     = "YOUR_IOS_GAME_ID";
    public string gameIdAndroid = "YOUR_ANDROID_GAME_ID";

    [Header("広告ユニット ID（デフォルトのまま変えなくてOK）")]
    public string interstitialAdUnitId = "Interstitial_Android";

    [Header("テストモード（リリース時は false に）")]
    public bool testMode = true;

    private bool _isInitialized = false;
    private bool _isAdLoaded    = false;

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

        InitializeAds();
    }

    void InitializeAds()
    {
        string gameId = Application.platform == RuntimePlatform.IPhonePlayer
            ? gameIdIos
            : gameIdAndroid;

        // 既に初期化済みなら不要
        if (Advertisement.isInitialized) { _isInitialized = true; LoadInterstitial(); return; }

        Advertisement.Initialize(gameId, testMode, this);
    }

    // ---- 外部 API ----

    /// <summary>
    /// ゲームオーバー時などに呼ぶ。ロード済みなら即表示、未ロードなら次回ゲームオーバーで表示。
    /// </summary>
    public void ShowInterstitial()
    {
        if (!_isInitialized || !_isAdLoaded)
        {
            Debug.Log("[AdsManager] Ad not ready yet.");
            return;
        }

        Advertisement.Show(interstitialAdUnitId, this);
    }

    // ---- IUnityAdsInitializationListener ----

    public void OnInitializationComplete()
    {
        Debug.Log("[AdsManager] Initialized.");
        _isInitialized = true;
        LoadInterstitial();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Initialization failed: {error} - {message}");
    }

    // ---- Load ----

    void LoadInterstitial()
    {
        _isAdLoaded = false;
#if UNITY_IOS
        interstitialAdUnitId = "Interstitial_iOS";
#endif
        Advertisement.Load(interstitialAdUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"[AdsManager] Ad loaded: {adUnitId}");
        _isAdLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Load failed: {adUnitId} - {error} - {message}");
    }

    // ---- Show ----

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"[AdsManager] Show complete: {adUnitId} ({showCompletionState})");
        // 次回のために再ロード
        LoadInterstitial();
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Show failed: {adUnitId} - {error} - {message}");
        LoadInterstitial();
    }

    public void OnUnityAdsShowStart(string adUnitId)     { }
    public void OnUnityAdsShowClick(string adUnitId)     { }
}
