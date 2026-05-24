using UnityEngine;

/// <summary>
/// World Space 캔버스를 카메라 앞 특정 위치에 배치하고
/// 항상 카메라를 향하도록 회전시킵니다.
/// </summary>
public class HW21_WorldSpaceCanvas : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform cameraTransform;
    public float distanceFromCamera = 1.5f;
    public float heightOffset = 0f;

    [Header("Billboard (카메라를 항상 바라봄)")]
    public bool billboard = true;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;

        PlaceInFrontOfCamera();
    }

    void Update()
    {
        if (billboard && cameraTransform != null)
            transform.LookAt(transform.position + (transform.position - cameraTransform.position));
    }

    private void PlaceInFrontOfCamera()
    {
        if (cameraTransform == null) return;
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();
        transform.position = cameraTransform.position
            + forward * distanceFromCamera
            + Vector3.up * heightOffset;
    }
}
