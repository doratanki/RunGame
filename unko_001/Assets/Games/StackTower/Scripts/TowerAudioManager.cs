using UnityEngine;

/// <summary>
/// Stack Tower の BGM / SE を管理する。
/// TowerGameManager と同じ GameObject にアタッチする。
/// Inspector で各 AudioClip をアサインする。
/// </summary>
public class TowerAudioManager : MonoBehaviour
{
    public static TowerAudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 0.6f;

    [Header("SE")]
    public AudioClip blockPlaceSE;   // 通常設置
    public AudioClip perfectSE;      // パーフェクト
    public AudioClip gameOverSE;     // ゲームオーバー

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

        // BGM 用 AudioSource
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = bgmVolume;

        // SE 用 AudioSource
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
        _bgmSource.volume = bgmVolume;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
            _bgmSource.Stop();
    }

    // ---- SE ----

    public void PlayBlockPlace()  => PlaySE(blockPlaceSE);
    public void PlayPerfect()     => PlaySE(perfectSE);
    public void PlayGameOver()    => PlaySE(gameOverSE);

    private void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        _seSource.PlayOneShot(clip, seVolume);
    }
}
