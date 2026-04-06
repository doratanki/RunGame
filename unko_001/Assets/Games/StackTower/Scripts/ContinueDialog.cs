using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Continue dialog.
/// The player chooses to continue or give up during the countdown.
/// Time-out is treated as a give-up automatically.
/// </summary>
public class ContinueDialog : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Text")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI scoreText;

    [Header("Buttons")]
    public Button continueButton;
    public Button giveUpButton;

    [Tooltip("Countdown duration in seconds. No countdown if 0 or less.")]
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

    // ---- Button OnClick ----

    public void OnContinueButton()
    {
        // Stop countdown and disable buttons while waiting for the ad
        if (_countdown != null)
        {
            StopCoroutine(_countdown);
            _countdown = null;
        }
        SetButtonsInteractable(false);

        // Skip the ad if ads have been removed by purchase
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
                // Reset countdown text so stale "0" or "1" isn't shown when restarting
                if (countdownText != null)
                    countdownText.text = Mathf.CeilToInt(countdownSeconds).ToString();

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

    // ---- Countdown ----

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

        // Time-out → give up
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
