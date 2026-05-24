using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Drag: 화면 드래그로 AR 공간에서 오브젝트를 이동합니다.
/// </summary>
public class HW20_DragInteraction : MonoBehaviour
{
    [Header("Drag Settings")]
    public float minY = 0f;
    public float maxY = 0.5f;

    private Camera _arCamera;
    private bool _isDragging = false;
    private Plane _dragPlane;
    private Vector3 _dragOffset;

    void Start()
    {
        _arCamera = Camera.main;
    }

    void Update()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
            TryBeginDrag(pointer.position.ReadValue());
        else if (pointer.press.isPressed && _isDragging)
            DragTo(pointer.position.ReadValue());
        else if (pointer.press.wasReleasedThisFrame)
            _isDragging = false;
    }

    void TryBeginDrag(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            _isDragging = true;
            _dragPlane = new Plane(Vector3.up, transform.position);
            if (_dragPlane.Raycast(ray, out float dist))
                _dragOffset = transform.position - ray.GetPoint(dist);
        }
    }

    void DragTo(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (_dragPlane.Raycast(ray, out float dist))
        {
            Vector3 newPos = ray.GetPoint(dist) + _dragOffset;
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            transform.position = newPos;
        }
    }
}
