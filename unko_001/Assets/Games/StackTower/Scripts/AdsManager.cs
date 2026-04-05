using System;
using System.Collections;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Advertisements;


/// <summary>
/// Unity Ads (UGS) を管理するシングルトン。
/// TowerGameManager と同じ GameObject にアタッチする。
///
/// 使い方:
///   1. Inspector で gameIdIos / gameIdAndroid を設定
///   2. TowerGameManager.OnGameOver() の末尾で AdsManager.Instance?.ShowInterstitial() を呼ぶ
///
/// 必要パッケージ:
///   - com.unity.services.core
///   - com.unity.ads (4.x)
/// </summary>
public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance { get; private set; }

    private const string GameIdIos     = "6082458";
    private const string GameIdAndroid = "6082459";
    private const string InterstitialAdUnitIdIos     = "Interstitial_iOS";
    private const string InterstitialAdUnitIdAndroid = "Interstitial_Android";
    private const string RewardedAdUnitIdIos         = "Rewarded_iOS";
    private const string RewardedAdUnitIdAndroid     = "Rewarded_Android";
    private const bool   TestMode = true;

    private string interstitialAdUnitId;
    private string rewardedAdUnitId;

    private bool _isInitialized    = false;
    private bool _isAdLoaded       = false;
    private bool _isRewardedLoaded = false;

    // リワード広告完了コールバック
    private Action _onRewardedComplete;
    private Action _onRewardedFailed;

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

        StartCoroutine(InitializeUGSCoroutine());
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
        string gameId = Application.platform == RuntimePlatform.IPhonePlayer
            ? GameIdIos
            : GameIdAndroid;

        interstitialAdUnitId = Application.platform == RuntimePlatform.IPhonePlayer
            ? InterstitialAdUnitIdIos
            : InterstitialAdUnitIdAndroid;

        rewardedAdUnitId = Application.platform == RuntimePlatform.IPhonePlayer
            ? RewardedAdUnitIdIos
            : RewardedAdUnitIdAndroid;

        if (Advertisement.isInitialized) { _isInitialized = true; LoadInterstitial(); LoadRewarded(); return; }

        Advertisement.Initialize(gameId, TestMode, this);
    }

    // ---- 外部 API ----

    /// <summary>
    /// ゲームオーバー時などに呼ぶ。ロード済みなら即表示、未ロードなら次回ゲームオーバーで表示。
    /// </summary>
    public void ShowInterstitial()
    {
        // 広告削除購入済みの場合はスキップ
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

        Advertisement.Show(interstitialAdUnitId, this);
    }

    // ---- IUnityAdsInitializationListener ----

    /// <summary>
    /// リワード広告を表示する。
    /// 視聴完了で onComplete、スキップ・失敗で onFailed を呼ぶ。
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
        Advertisement.Show(rewardedAdUnitId, this);
    }

    public bool IsRewardedReady => _isInitialized && _isRewardedLoaded;

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
        Advertisement.Load(interstitialAdUnitId, this);
    }

    void LoadRewarded()
    {
        _isRewardedLoaded = false;
        Advertisement.Load(rewardedAdUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"[AdsManager] Ad loaded: {adUnitId}");
        if (adUnitId == rewardedAdUnitId)
            _isRewardedLoaded = true;
        else
            _isAdLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Load failed: {adUnitId} - {message}");
    }

    // ---- Show ----

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"[AdsManager] Show complete: {adUnitId} ({showCompletionState})");

        if (adUnitId == rewardedAdUnitId)
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
        Debug.LogWarning($"[AdsManager] Show failed: {adUnitId} - {message}");

        if (adUnitId == rewardedAdUnitId)
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

    public void OnUnityAdsShowStart(string adUnitId)  { }
    public void OnUnityAdsShowClick(string adUnitId)  { }
}
