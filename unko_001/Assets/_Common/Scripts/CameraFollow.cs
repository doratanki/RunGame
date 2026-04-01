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
    public Vector3 offset = new Vector3(5f, 5f, -10f);

    [Header("スムーズ速度")]
    public float smoothSpeed = 4f;

    void LateUpdate()
    {
        if (target == null) return;

        // X / Z はオフセット固定、Y のみターゲットに追従
        float targetY = target.position.y + offset.y;
        Vector3 desiredPos = new Vector3(offset.x, targetY, offset.z);

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(new Vector3(0f, target.position.y, 0f));
    }
}
