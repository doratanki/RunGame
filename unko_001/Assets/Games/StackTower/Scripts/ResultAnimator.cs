using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// スコアのカウントアップとランク遷移演出を駆動するコルーチン管理クラス。
/// </summary>
public class ResultAnimator : MonoBehaviour
{
    [Tooltip("カウントアップにかける秒数")]
    public float countDuration = 2.0f;

    [Tooltip("ランクアップ時に挿入するポーズ秒数")]
    public float rankUpPauseDuration = 0.3f;

    private Coroutine _running;

    /// <summary>
    /// スコアを 0 から finalScore までカウントアップしながら、
    /// ランク閾値を超えるたびに onRankChanged を呼ぶ。
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

            // イーズアウト（終盤ゆっくり）
            float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            int displayScore = Mathf.RoundToInt(eased * finalScore);

            onScoreUpdated?.Invoke(displayScore);

            // ランク閾値を越えたか確認
            RankEntry newRank = RankCalculator.GetRank(rankTable, displayScore);
            if (newRank != null && newRank.label != currentRank?.label)
            {
                currentRank = newRank;
                onRankChanged?.Invoke(currentRank);

                // ランクアップ時は一瞬ポーズして演出を目立たせる
                yield return new WaitForSeconds(rankUpPauseDuration);
                elapsed += rankUpPauseDuration; // ポーズ分を経過時間に加算
            }

            yield return null;
        }

        // 最終値を確定
        onScoreUpdated?.Invoke(finalScore);
        RankEntry finalRank = RankCalculator.GetRank(rankTable, finalScore);
        if (finalRank != null && finalRank.label != currentRank?.label)
            onRankChanged?.Invoke(finalRank);

        _running = null;
        onComplete?.Invoke();
    }
}
