using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの自動前進・左右ドラッグ操作・仲間リスト管理。
/// </summary>
public class RunnerPlayer : MonoBehaviour
{
    [Header("移動")]
    public float forwardSpeed = 8f;
    public float lateralSensitivity = 0.03f;  // ドラッグ量 → X 移動量の係数
    public float laneWidth = 4f;               // X 軸の移動範囲（±）

    [Header("仲間整列")]
    public float followSpacing = 0.8f;   // 仲間同士の前後間隔
    public float followSpeed   = 10f;    // 仲間の追従速度

    public bool IsRunning { get; private set; } = false;

    // 収集済みの仲間リスト（順番 = 整列順）
    private readonly List<CrowdMember> _members = new List<CrowdMember>();

    private Vector2 _prevTouchPos;
    private bool _isTouching = false;

    // ---- 公開 API ----

    public void StartRunning()
    {
        IsRunning = true;
    }

    public void StopRunning()
    {
        IsRunning = false;
    }

    public void AddMember(CrowdMember member)
    {
        member.Collect(this, _members.Count);
        _members.Add(member);
        CrowdGameManager.Instance?.AddMember();
    }

    public void RemoveMembers(int count)
    {
        int removeCount = Mathf.Min(count, _members.Count);
        for (int i = 0; i < removeCount; i++)
        {
            int last = _members.Count - 1;
            _members[last].Scatter();
            _members.RemoveAt(last);
        }
        // 残った仲間のインデックスを再割り当て
        for (int i = 0; i < _members.Count; i++)
            _members[i].SetIndex(i);

        CrowdGameManager.Instance?.RemoveMember(removeCount);
    }

    // ---- Unity ライフサイクル ----

    void Update()
    {
        if (!IsRunning) return;

        MoveForward();
        HandleInput();
        ClampLateral();
    }

    void MoveForward()
    {
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    void HandleInput()
    {
        float deltaX = 0f;

        // タッチ入力
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _prevTouchPos = touch.position;
                _isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && _isTouching)
            {
                deltaX = (touch.position.x - _prevTouchPos.x) * lateralSensitivity;
                _prevTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isTouching = false;
            }
        }
        // マウス入力（Editor / PC）
        else if (Input.GetMouseButton(0))
        {
            deltaX = Input.GetAxis("Mouse X") * lateralSensitivity * 10f;
        }

        transform.position += Vector3.right * deltaX;
    }

    void ClampLateral()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -laneWidth, laneWidth);
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        CrowdMember member = other.GetComponent<CrowdMember>();
        if (member != null && !member.IsCollected)
        {
            AddMember(member);
            return;
        }

        Obstacle obstacle = other.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            RemoveMembers(obstacle.lossCount);
        }
    }
}
