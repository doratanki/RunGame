using UnityEngine;

/// <summary>
/// Camera controller that follows only the target's Y position.
/// Used for vertically stacking games like Stack Tower.
/// X / Z are fixed by offset only.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Offset (relative camera position)")]
    public Vector3 offset = new Vector3(7f, 7f, -20f);

    [Header("Smooth Speed")]
    public float smoothSpeed = 4f;

    [HideInInspector] public Vector3 shakeOffset;

    /// <summary>Snaps the camera to the target immediately on game reset.</summary>
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

        // X / Z are fixed by offset, only Y follows the target
        float targetY = target.position.y + offset.y;
        Vector3 desiredPos = new Vector3(offset.x, targetY, offset.z) + shakeOffset;

        float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPos, t);
        transform.LookAt(new Vector3(0f, target.position.y, 0f));
    }
}
