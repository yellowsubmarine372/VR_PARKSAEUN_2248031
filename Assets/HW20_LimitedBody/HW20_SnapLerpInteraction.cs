using UnityEngine;

/// <summary>
/// Snap + Lerp: 오브젝트를 드래그하여 놓으면 가장 가까운 그리드 위치로 부드럽게 스냅됩니다.
/// 공간의 규칙성(그리드)을 통해 사용자가 현실 공간을 인식하게 합니다.
/// </summary>
public class HW20_SnapLerpInteraction : MonoBehaviour
{
    [Header("Grid Snap Settings")]
    public float gridSize = 0.05f;
    public float lerpSpeed = 8f;

    [Header("Snap Points (Optional)")]
    public Transform[] snapPoints;
    public float snapRadius = 0.08f;

    private Camera _arCamera;
    private bool _isDragging = false;
    private bool _isSnapping = false;
    private Vector3 _snapTarget;
    private Plane _dragPlane;

    void Start()
    {
        _arCamera = Camera.main;
        _snapTarget = transform.position;
    }

    void Update()
    {
        if (_isSnapping)
        {
            transform.position = Vector3.Lerp(transform.position, _snapTarget, Time.deltaTime * lerpSpeed);
            if (Vector3.Distance(transform.position, _snapTarget) < 0.001f)
            {
                transform.position = _snapTarget;
                _isSnapping = false;
            }
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) TryBeginDrag(touch.position);
            else if (touch.phase == TouchPhase.Moved && _isDragging) DragTo(touch.position);
            else if (touch.phase == TouchPhase.Ended && _isDragging) EndDrag();
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) TryBeginDrag(Input.mousePosition);
        else if (Input.GetMouseButton(0) && _isDragging) DragTo(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0) && _isDragging) EndDrag();
#endif
    }

    void TryBeginDrag(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            _isDragging = true;
            _isSnapping = false;
            _dragPlane = new Plane(Vector3.up, transform.position);
        }
    }

    void DragTo(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (_dragPlane.Raycast(ray, out float dist))
            transform.position = ray.GetPoint(dist);
    }

    void EndDrag()
    {
        _isDragging = false;
        _snapTarget = GetSnapPosition(transform.position);
        _isSnapping = true;
    }

    Vector3 GetSnapPosition(Vector3 pos)
    {
        // 명시적 스냅 포인트가 있으면 가장 가까운 포인트로 스냅
        if (snapPoints != null && snapPoints.Length > 0)
        {
            Transform closest = null;
            float minDist = float.MaxValue;
            foreach (var sp in snapPoints)
            {
                if (sp == null) continue;
                float d = Vector3.Distance(pos, sp.position);
                if (d < minDist) { minDist = d; closest = sp; }
            }
            if (closest != null && minDist < snapRadius)
                return closest.position;
        }

        // 그리드 기반 스냅
        float x = Mathf.Round(pos.x / gridSize) * gridSize;
        float y = pos.y;
        float z = Mathf.Round(pos.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }
}
