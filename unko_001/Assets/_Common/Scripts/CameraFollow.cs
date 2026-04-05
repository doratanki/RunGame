using UnityEngine;

/// <summary>
/// ターゲットの Y 座標にのみ追従するカメラ制御。
/// Stack Tower のように縦に積み上げるゲームで使用する。
/// X / Z はオフセットのみ固定。
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("オフセット（カメラ位置の相対値）")]
    public Vector3 offset = new Vector3(7f, 7f, -20f);

    [Header("スムーズ速度")]
    public float smoothSpeed = 4f;

    [HideInInspector] public Vector3 shakeOffset;

    /// <summary>ゲームリセット時にカメラ位置を即スナップする。</summary>
    public void SnapToTarget()
    {
        if (target == null) return;
        float targetY = target.position.y + offset.y;
        transform.position = new Vector3(offset.x, targetY, offset.z);
        transform.LookAt(new Vector3(0f, target.position.y, 0f));
    }

    void LateUpdate()
    {
        if (target == null) return;

        // X / Z はオフセット固定、Y のみターゲットに追従
        float targetY = target.position.y + offset.y;
        Vector3 desiredPos = new Vector3(offset.x, targetY, offset.z) + shakeOffset;

        float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, t);
        transform.LookAt(new Vector3(0f, target.position.y, 0f));
    }
}
