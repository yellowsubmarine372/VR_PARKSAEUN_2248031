using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Tap: 화면 터치 시 오브젝트의 색상과 크기를 토글합니다.
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

        var pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
            HandleTap(pointer.position.ReadValue());
    }

    void HandleTap(Vector2 screenPos)
    {
        if (_arCamera == null) return;
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            _isTapped = !_isTapped;
            _targetScale = _isTapped ? tappedScale : normalScale;
            if (_renderer != null)
                _renderer.material.color = _isTapped ? tappedColor : normalColor;
        }
    }
}
