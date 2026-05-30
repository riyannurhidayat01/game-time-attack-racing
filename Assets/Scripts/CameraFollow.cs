using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0, 12, -18);
    public float smoothSpeed = 5f;
    public float lookHeight = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition =
            target.position +
            target.up * offset.y +
            target.forward * offset.z +
            target.right * offset.x;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}