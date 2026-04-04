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
    private const bool   TestMode = true;

    private string interstitialAdUnitId;

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

        if (Advertisement.isInitialized) { _isInitialized = true; LoadInterstitial(); return; }

        Advertisement.Initialize(gameId, TestMode, this);
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
        Advertisement.Load(interstitialAdUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"[AdsManager] Ad loaded: {adUnitId}");
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
        LoadInterstitial();
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"[AdsManager] Show failed: {adUnitId} - {message}");
        LoadInterstitial();
    }

    public void OnUnityAdsShowStart(string adUnitId)  { }
    public void OnUnityAdsShowClick(string adUnitId)  { }
}
