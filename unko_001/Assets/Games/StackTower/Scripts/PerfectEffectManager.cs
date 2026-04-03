using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// パーフェクト時のエフェクトをまとめて管理する。
/// TowerGameManager と同じ GameObject にアタッチし、PlayPerfect() を呼ぶ。
/// </summary>
public class PerfectEffectManager : MonoBehaviour
{
    public static PerfectEffectManager Instance { get; private set; }

    [Header("1. 画面フラッシュ")]
    public Image flashImage;
    public Color flashColor = new Color(1f, 1f, 0.6f, 0.6f);
    public float flashDuration = 0.25f;

    [Header("2. カメラシェイク")]
    public CameraFollow cameraFollow;
    public float shakeDuration  = 0.3f;
    public float shakeMagnitude = 0.15f;

    [Header("3. ビネット")]
    public Image vignetteImage;
    public Color vignetteColor = new Color(1f, 0.85f, 0f, 0.5f);
    public float vignetteDuration = 0.4f;

    [Header("4. パーティクル")]
    public ParticleSystem perfectParticle;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        if (cameraFollow != null)
            cameraFollow.shakeOffset = Vector3.zero;
    }

    public void PlayPerfect(Vector3 worldPos)
    {
        if (flashImage    != null) StartCoroutine(FlashRoutine());
        if (cameraFollow  != null) StartCoroutine(ShakeRoutine());
        if (vignetteImage != null) StartCoroutine(VignetteRoutine());
        if (perfectParticle != null)
        {
            perfectParticle.transform.position = worldPos;
            perfectParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            perfectParticle.Play();
        }
    }

    // ---- 1. フラッシュ ----
    IEnumerator FlashRoutine()
    {
        flashImage.color = flashColor;
        flashImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            var c = flashColor;
            c.a = Mathf.Lerp(flashColor.a, 0f, elapsed / flashDuration);
            flashImage.color = c;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    // ---- 2. カメラシェイク ----
    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = Mathf.Lerp(shakeMagnitude, 0f, elapsed / shakeDuration);
            cameraFollow.shakeOffset = Random.insideUnitSphere * strength;
            yield return null;
        }
        cameraFollow.shakeOffset = Vector3.zero;
    }

    // ---- 3. ビネット ----
    IEnumerator VignetteRoutine()
    {
        vignetteImage.color = vignetteColor;
        vignetteImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < vignetteDuration)
        {
            elapsed += Time.deltaTime;
            var c = vignetteColor;
            c.a = Mathf.Lerp(vignetteColor.a, 0f, elapsed / vignetteDuration);
            vignetteImage.color = c;
            yield return null;
        }

        vignetteImage.gameObject.SetActive(false);
    }
}
