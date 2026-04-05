using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// コンティニューダイアログ。
/// カウントダウン中に「続ける」か「あきらめる」を選ぶ。
/// 時間切れは自動的に GiveUp 扱い。
/// </summary>
public class ContinueDialog : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("テキスト")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI scoreText;

    [Header("ボタン")]
    public Button continueButton;
    public Button giveUpButton;

    [Tooltip("カウントダウン秒数。0 以下でカウントダウンなし")]
    public float countdownSeconds = 5f;

    Coroutine _countdown;

    public void Show(int currentScore)
    {
        if (panel != null) panel.SetActive(true);

        if (scoreText != null)
            scoreText.text = "SCORE  " + currentScore;

        if (_countdown != null) StopCoroutine(_countdown);

        if (countdownSeconds > 0f)
            _countdown = StartCoroutine(RunCountdown());
        else if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    public void Hide()
    {
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }
        if (panel != null) panel.SetActive(false);
    }

    // ---- ボタン OnClick ----

    public void OnContinueButton()
    {
        // カウントダウンを止めて広告を待つ間は操作不可に
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }
        SetButtonsInteractable(false);

        // 課金済みなら広告なしでそのままコンティニュー
        if (IAPManager.Instance != null && IAPManager.Instance.IsRemoveAdsPurchased)
        {
            Hide();
            TowerGameManager.Instance?.ContinueGame();
            return;
        }

        AdsManager.Instance?.ShowRewarded(
            onComplete: () =>
            {
                Hide();
                TowerGameManager.Instance?.ContinueGame();
            },
            onFailed: () =>
            {
                // 広告をスキップ・失敗した場合は再度選択させる
                SetButtonsInteractable(true);
                if (countdownSeconds > 0f)
                    _countdown = StartCoroutine(RunCountdown());
            }
        );
    }

    public void OnGiveUpButton()
    {
        Hide();
        TowerGameManager.Instance?.GiveUp();
    }

    // ---- カウントダウン ----

    IEnumerator RunCountdown()
    {
        SetButtonsInteractable(true);
        float remaining = countdownSeconds;

        while (remaining > 0f)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();

            remaining -= Time.deltaTime;
            yield return null;
        }

        // 時間切れ → あきらめる
        if (countdownText != null)
            countdownText.text = "0";

        SetButtonsInteractable(false);
        _countdown = null;
        Hide();
        TowerGameManager.Instance?.GiveUp();
    }

    void SetButtonsInteractable(bool value)
    {
        if (continueButton != null) continueButton.interactable = value;
        if (giveUpButton   != null) giveUpButton.interactable   = value;
    }
}
