using UnityEngine;

/// <summary>
/// Crowd Runner 専用カメラ。target の Z 座標のみ追従し、X/Y はオフセット固定。
/// </summary>
public class RunnerCamera : MonoBehaviour
{
    public Transform target;

    [Header("オフセット")]
    public Vector3 offset = new Vector3(0f, 8f, -10f);

    [Header("スムーズ速度")]
    public float smoothSpeed = 6f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(offset.x, offset.y, target.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // プレイヤーのY高さを基準にやや前方を見下ろす
        transform.LookAt(new Vector3(0f, target.position.y, target.position.z + 3f));
    }
}
