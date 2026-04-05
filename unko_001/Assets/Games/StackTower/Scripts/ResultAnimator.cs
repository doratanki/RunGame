using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Coroutine manager that drives the score count-up and rank transition animation.
/// </summary>
public class ResultAnimator : MonoBehaviour
{
    [Tooltip("Duration of the count-up animation in seconds")]
    public float countDuration = 2.0f;

    [Tooltip("Pause duration inserted on rank-up to make the effect visible")]
    public float rankUpPauseDuration = 0.3f;

    private Coroutine _running;

    /// <summary>
    /// Counts up from 0 to finalScore, calling onRankChanged each time a rank threshold is crossed.
    /// </summary>
    public void Animate(
        int finalScore,
        RankTable rankTable,
        Action<int> onScoreUpdated,
        Action<RankEntry> onRankChanged,
        Action onComplete = null)
    {
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(
            RunAnimation(finalScore, rankTable, onScoreUpdated, onRankChanged, onComplete));
    }

    public void Stop()
    {
        if (_running != null)
        {
            StopCoroutine(_running);
            _running = null;
        }
    }

    IEnumerator RunAnimation(
        int finalScore,
        RankTable rankTable,
        Action<int> onScoreUpdated,
        Action<RankEntry> onRankChanged,
        Action onComplete)
    {
        RankEntry currentRank = RankCalculator.GetRank(rankTable, 0);
        onScoreUpdated?.Invoke(0);
        onRankChanged?.Invoke(currentRank);

        float elapsed = 0f;

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countDuration;

            // Ease-out (slows down near the end)
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            int displayScore = Mathf.RoundToInt(eased * finalScore);

            onScoreUpdated?.Invoke(displayScore);

            // Check if a rank threshold was crossed
            RankEntry newRank = RankCalculator.GetRank(rankTable, displayScore);
            if (newRank != null && newRank.label != currentRank?.label)
            {
                currentRank = newRank;
                onRankChanged?.Invoke(currentRank);

                // Brief pause on rank-up to highlight the effect
                yield return new WaitForSeconds(rankUpPauseDuration);
                elapsed += rankUpPauseDuration; // Add pause time to elapsed so duration stays accurate
            }

            yield return null;
        }

        // Finalize
        onScoreUpdated?.Invoke(finalScore);
        RankEntry finalRank = RankCalculator.GetRank(rankTable, finalScore);
        if (finalRank != null && finalRank.label != currentRank?.label)
            onRankChanged?.Invoke(finalRank);

        _running = null;
        onComplete?.Invoke();
    }
}
