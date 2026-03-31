using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float laneDistance = 2f;
    public float laneSwitchSpeed = 8f;

    [Header("Number")]
    public float currentNumber = 1f;
    public TextMeshPro numberText;

    [Header("State")]
    public bool isRunning = false;

    private int targetLane = 0; // -1=left, 0=center, 1=right
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    private const float SwipeThreshold = 50f;

    void Update()
    {
        if (!isRunning) return;

        // Auto forward
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;

        // Input
        HandleTouchInput();
        HandleKeyboardInput();

        // Lane movement
        float targetX = targetLane * laneDistance;
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, targetX, laneSwitchSpeed * Time.deltaTime);
        transform.position = pos;

        // Billboard number text
        if (numberText != null)
        {
            numberText.transform.rotation = Quaternion.LookRotation(
                numberText.transform.position - Camera.main.transform.position
            );
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        float deltaX = touch.position.x - touchStartPos.x;
                        if (Mathf.Abs(deltaX) > SwipeThreshold)
                        {
                            if (deltaX > 0)
                                MoveLane(1);
                            else
                                MoveLane(-1);
                            isSwiping = false;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveLane(1);
    }

    void MoveLane(int direction)
    {
        targetLane = Mathf.Clamp(targetLane + direction, -1, 1);
    }

    public void ApplyOperation(OperationType op, float operand)
    {
        switch (op)
        {
            case OperationType.Add:
                currentNumber += operand;
                break;
            case OperationType.Subtract:
                currentNumber -= operand;
                break;
            case OperationType.Multiply:
                currentNumber *= operand;
                break;
            case OperationType.Divide:
                if (operand != 0)
                    currentNumber /= operand;
                break;
        }

        currentNumber = Mathf.Max(0f, currentNumber);
        currentNumber = Mathf.Min(999999f, currentNumber);
        UpdateNumberDisplay();
    }

    public void UpdateNumberDisplay()
    {
        if (numberText != null)
            numberText.text = Mathf.RoundToInt(currentNumber).ToString();
    }

    public void StartRunning()
    {
        isRunning = true;
        targetLane = 0;
        UpdateNumberDisplay();
    }

    public void StopRunning()
    {
        isRunning = false;
    }
}
