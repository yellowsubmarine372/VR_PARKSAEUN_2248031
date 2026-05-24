using UnityEngine;

/// <summary>
/// Drag: 화면 드래그로 AR 공간에서 오브젝트를 이동합니다.
/// GroundPlane 위에서 손가락 하나로 오브젝트 위치를 바꿉니다.
/// </summary>
public class HW20_DragInteraction : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 0.01f;
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
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) TryBeginDrag(touch.position);
            else if (touch.phase == TouchPhase.Moved && _isDragging) DragTo(touch.position);
            else if (touch.phase == TouchPhase.Ended) _isDragging = false;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) TryBeginDrag(Input.mousePosition);
        else if (Input.GetMouseButton(0) && _isDragging) DragTo(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0)) _isDragging = false;
#endif
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
