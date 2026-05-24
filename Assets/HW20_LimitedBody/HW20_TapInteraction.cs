using UnityEngine;
using Vuforia;

/// <summary>
/// Tap: 화면 터치 시 오브젝트의 색상과 크기를 토글합니다.
/// 사용자가 터치만으로 가상 오브젝트의 '존재'를 확인하게 합니다.
/// </summary>
public class HW20_TapInteraction : MonoBehaviour
{
    [Header("Tap Settings")]
    public Color normalColor = Color.white;
    public Color tappedColor = Color.cyan;
    public Vector3 normalScale = Vector3.one;
    public Vector3 tappedScale = new Vector3(1.3f, 1.3f, 1.3f);
    public float scaleSpeed = 5f;

    private Renderer _renderer;
    private bool _isTapped = false;
    private Vector3 _targetScale;
    private Camera _arCamera;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _renderer.material.color = normalColor;
        _targetScale = normalScale;
        _arCamera = Camera.main;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * scaleSpeed);

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTap(Input.GetTouch(0).position);
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
#endif
    }

    void HandleTap(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                _isTapped = !_isTapped;
                _targetScale = _isTapped ? tappedScale : normalScale;
                if (_renderer != null)
                    _renderer.material.color = _isTapped ? tappedColor : normalColor;
            }
        }
    }
}
