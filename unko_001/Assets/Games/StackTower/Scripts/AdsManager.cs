using System;
using System.Collections;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Manages Unity Ads (UGS) as a singleton.
/// Attach to the same GameObject as TowerGameManager.
///
/// Usage:
///   1. Set gameIdIos / gameIdAndroid in the Inspector
///   2. Call AdsManager.Instance?.ShowInterstitial() at the end of TowerGameManager.OnGameOver()
///
/// Required packages:
///   - com.unity.services.core
///   - com.unity.ads (4.x)
/// </summary>
public class AdsManager : Singleton<AdsManager>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Game IDs (set per platform in Inspector)")]
    [SerializeField] private string gameIdIos     = "YOUR_IOS_GAME_ID";
    [SerializeField] private string gameIdAndroid = "YOUR_ANDROID_GAME_ID";

    [Header("Ad Unit IDs")]
    [SerializeField] private string interstitialAdUnitIdIos     = "Interstitial_iOS";
    [SerializeField] private string interstitialAdUnitIdAndroid = "Interstitial_Android";
    [SerializeField] private string rewardedAdUnitIdIos         = "Rewarded_iOS";
    [SerializeField] private string rewardedAdUnitIdAndroid     = "Rewarded_Android";

    [Header("Settings")]
    [SerializeField] private bool testMode = true;
    [Tooltip("Seconds before IsAdShowing is force-reset if no callback arrives.")]
    [SerializeField] private float adShowTimeout = 10f;

    private string _interstitialAdUnitId;
    private string _rewardedAdUnitId;

    private bool _isInitialized    = false;
    private bool _isAdLoaded       = false;
    private bool _isRewardedLoaded = false;

    /// <summary>True while an ad is showing. Use this to block game input.</summary>
    public bool IsAdShowing { get; private set; } = false;

    private float _adShowTimeoutRemaining = -1f;

    // Rewarded ad completion callbacks
    private Action _onRewardedComplete;
    private Action _onRewardedFailed;

    // ---- Lifecycle ----

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        StartCoroutine(InitializeUGSCoroutine());
    }

    void Update()
    {
        // Safety timeout: if IsAdShowing gets stuck, reset it after adShowTimeout seconds
        if (_adShowTimeoutRemaining > 0f)
        {
            _adShowTimeoutRemaining -= Time.deltaTime;
            if (_adShowTimeoutRemaining <= 0f)
            {
                Debug.LogWarning("[AdsManager] Ad show timeout — resetting IsAdShowing flag.");
                IsAdShowing = false;
                _adShowTimeoutRemaining = -1f;
            }
        }
    }

    IEnumerator InitializeUGSCoroutine()
    {
        var initTask = UnityServices.InitializeAsync();
        yield return new WaitUntil(() => initTask.IsCompleted);

        if (initTask.IsFaulted)
        {
            Debug.LogWarning($"[AdsManager] UGS initialization failed: {initTask.Exception?.Message}");
            yield break;
        }

        InitializeAds();
    }

    void InitializeAds()
    {
        bool isIos = Application.platform == RuntimePlatform.IPhonePlayer;
        string gameId = isIos ? gameIdIos : gameIdAndroid;

        _interstitialAdUnitId = isIos ? interstitialAdUnitIdIos : interstitialAdUnitIdAndroid;
        _rewardedAdUnitId     = isIos ? rewardedAdUnitIdIos     : rewardedAdUnitIdAndroid;

        if (Advertisement.isInitialized)
        {
            _isInitialized = true;
            LoadInterstitial();
            LoadRewarded();
            return;
        }

        Advertisement.Initialize(gameId, testMode, this);
    }

    // ---- Public API ----

    /// <summary>
    /// Call on game over etc. Shows immediately if loaded, otherwise skips until the next call.
    /// </summary>
    public void ShowInterstitial()
    {
        if (IAPManager.Instance != null && IAPManager.Instance.IsRemoveAdsPurchased)
        {
            Debug.Log("[AdsManager] Ads removed by purchase. Skipping.");
            return;
        }

        if (!_isInitialized || !_isAdLoaded)
        {
            Debug.Log("[AdsManager] Ad not ready yet.");
            return;
        }

        Advertisement.Show(_interstitialAdUnitId, this);
    }

    /// <summary>
    /// Show a rewarded ad.
    /// Calls onComplete on full view, onFailed on skip or failure.
    /// </summary>
    public void ShowRewarded(Action onComplete, Action onFailed = null)
    {
        if (!_isInitialized || !_isRewardedLoaded)
        {
            Debug.LogWarning("[AdsManager] Rewarded ad not ready.");
            onFailed?.Invoke();
            return;
        }
        _onRewardedComplete = onComplete;
        _onRewardedFailed   = onFailed;
        Advertisement.Show(_rewardedAdUnitId, this);
    }

    public bool IsRewardedReady => _isInitialized && _isRewardedLoaded;

    // ---- IUnityAdsInitializationListener ----

    public void OnInitializationComplete()
    {
        Debug.Log("[AdsManager] Initialized.");
        _isInitialized = true;
        LoadInterstitial();
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Initialization failed: {error} - {message}");
    }

    // ---- Load ----

    void LoadInterstitial()
    {
        _isAdLoaded = false;
        Advertisement.Load(_interstitialAdUnitId, this);
    }

    void LoadRewarded()
    {
        _isRewardedLoaded = false;
        Advertisement.Load(_rewardedAdUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"[AdsManager] Ad loaded: {adUnitId}");
        if (adUnitId == _rewardedAdUnitId)
            _isRewardedLoaded = true;
        else
            _isAdLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Load failed: {adUnitId} - {message}");
    }

    // ---- Show ----

    public void OnUnityAdsShowStart(string adUnitId)
    {
        IsAdShowing = true;
        _adShowTimeoutRemaining = adShowTimeout;
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        IsAdShowing = false;
        _adShowTimeoutRemaining = -1f;
        Debug.Log($"[AdsManager] Show complete: {adUnitId} ({showCompletionState})");

        if (adUnitId == _rewardedAdUnitId)
        {
            LoadRewarded();
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
                _onRewardedComplete?.Invoke();
            else
                _onRewardedFailed?.Invoke();
            _onRewardedComplete = null;
            _onRewardedFailed   = null;
        }
        else
        {
            LoadInterstitial();
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        IsAdShowing = false;
        _adShowTimeoutRemaining = -1f;
        Debug.LogWarning($"[AdsManager] Show failed: {adUnitId} - {message}");

        if (adUnitId == _rewardedAdUnitId)
        {
            LoadRewarded();
            _onRewardedFailed?.Invoke();
            _onRewardedComplete = null;
            _onRewardedFailed   = null;
        }
        else
        {
            LoadInterstitial();
        }
    }

    public void OnUnityAdsShowClick(string adUnitId) { }
}
