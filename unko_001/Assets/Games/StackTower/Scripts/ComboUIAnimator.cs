using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the PERFECT! popup fade-out animation.
/// Call PlayPerfect() from TowerUI to trigger it.
/// </summary>
public class ComboUIAnimator : MonoBehaviour
{
    public TextMeshProUGUI perfectText;

    [Header("Timing")]
    public float displayDuration = 0.6f;
    public float fadeDuration    = 0.4f;

    [Header("Colors")]
    public Color perfectColor = Color.white;
    public Color goodColor    = new Color(0.4f, 0.9f, 0.4f);
    public Color badColor     = new Color(0.9f, 0.4f, 0.4f);

    private Coroutine _currentAnim;

    public void PlayPerfect() => Play(perfectColor);
    public void PlayGood()    => Play(goodColor);
    public void PlayBad()     => Play(badColor);

    void Play(Color color)
    {
        if (_currentAnim != null)
            StopCoroutine(_currentAnim);
        _currentAnim = StartCoroutine(AnimRoutine(color));
    }

    IEnumerator AnimRoutine(Color textColor)
    {
        if (perfectText == null) yield break;

        perfectText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        perfectText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

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

    void OnDestroy()
    {
        if (_currentAnim != null)
            StopCoroutine(_currentAnim);
    }
}
