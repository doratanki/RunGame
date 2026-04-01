using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// PERFECT! ポップアップのフェードアウトアニメを管理する。
/// TowerUI から PlayPerfect() を呼ぶだけで動作する。
/// </summary>
public class ComboUIAnimator : MonoBehaviour
{
    public TextMeshProUGUI perfectText;

    [Header("タイミング")]
    public float displayDuration = 0.6f;
    public float fadeDuration    = 0.4f;

    private Coroutine _currentAnim;

    public void PlayPerfect()
    {
        if (_currentAnim != null)
            StopCoroutine(_currentAnim);
        _currentAnim = StartCoroutine(PerfectRoutine());
    }

    IEnumerator PerfectRoutine()
    {
        if (perfectText == null) yield break;

        // 完全不透明で表示
        SetAlpha(1f);
        perfectText.gameObject.SetActive(true);

        // 表示を維持
        yield return new WaitForSeconds(displayDuration);

        // フェードアウト
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(1f - elapsed / fadeDuration);
            yield return null;
        }

        SetAlpha(0f);
        perfectText.gameObject.SetActive(false);
        _currentAnim = null;
    }

    void SetAlpha(float alpha)
    {
        if (perfectText == null) return;
        var c = perfectText.color;
        c.a = alpha;
        perfectText.color = c;
    }
}
