using System.Collections;
using UnityEngine;

/// <summary>
/// Manages BGM and SFX for Stack Tower.
/// Attach to the same GameObject as TowerGameManager.
/// Assign each AudioClip in the Inspector.
/// </summary>
public class TowerAudioManager : MonoBehaviour
{
    public static TowerAudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 0.6f;
    public float bgmFadeDuration = 1f;

    [Header("SFX")]
    public AudioClip blockPlaceSE;   // Normal placement
    public AudioClip perfectSE;      // Perfect placement
    public AudioClip gameOverSE;     // Game over

    [Range(0f, 1f)] public float seVolume = 1f;

    private AudioSource _bgmSource;
    private AudioSource _seSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioSource for BGM
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = bgmVolume;

        // AudioSource for SFX
        _seSource = gameObject.AddComponent<AudioSource>();
        _seSource.loop = false;
        _seSource.playOnAwake = false;
        _seSource.volume = seVolume;
    }

    // ---- BGM ----

    public void PlayBGM()
    {
        if (bgmClip == null || _bgmSource.isPlaying) return;
        _bgmSource.clip = bgmClip;
        _bgmSource.volume = 0f;
        _bgmSource.Play();
        StartCoroutine(FadeBGM(0f, bgmVolume, bgmFadeDuration));
    }

    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
            StartCoroutine(FadeBGM(bgmVolume, 0f, bgmFadeDuration, stopOnComplete: true));
    }

    IEnumerator FadeBGM(float from, float to, float duration, bool stopOnComplete = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _bgmSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _bgmSource.volume = to;
        if (stopOnComplete) _bgmSource.Stop();
    }

    // ---- SFX ----

    public void PlayBlockPlace()  => PlaySE(blockPlaceSE);
    public void PlayPerfect()     => PlaySE(perfectSE);
    public void PlayGameOver()    => PlaySE(gameOverSE);

    private void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        _seSource.PlayOneShot(clip, seVolume);
    }
}
