using UnityEngine;
using TMPro;

public class BossZone : MonoBehaviour
{
    public float bossNumber = 100f;
    public TextMeshPro bossNumberText;

    private bool triggered = false;

    public void Initialize(float number)
    {
        bossNumber = number;
        if (bossNumberText != null)
            bossNumberText.text = Mathf.RoundToInt(bossNumber).ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            triggered = true;
            player.StopRunning();

            if (player.currentNumber >= bossNumber)
                GameManager.Instance.OnWin();
            else
                GameManager.Instance.OnLose();
        }
    }
}
