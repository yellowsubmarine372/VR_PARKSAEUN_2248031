using UnityEngine;
using Vuforia;

/// <summary>
/// ImageTarget 인식/소실 시 가상 오브젝트를 Lerp로 등장/퇴장시킵니다.
/// "실제 이미지가 인식될 때만 가상 세계가 열린다"는 현실 판단 실험.
/// </summary>
public class HW20_ImageTargetHandler : MonoBehaviour
{
    [Header("Objects to Show/Hide")]
    public GameObject[] virtualObjects;

    [Header("Lerp Settings")]
    public float appearSpeed = 3f;
    public float disappearSpeed = 5f;
    public Vector3 hiddenScale = Vector3.zero;
    public Vector3 visibleScale = Vector3.one;

    private ObserverBehaviour _observer;
    private bool _isTracked = false;

    void Start()
    {
        _observer = GetComponent<ObserverBehaviour>();
        if (_observer != null)
        {
            _observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        // 시작 시 오브젝트 숨김
        foreach (var obj in virtualObjects)
            if (obj != null) obj.transform.localScale = hiddenScale;
    }

    void OnDestroy()
    {
        if (_observer != null)
            _observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        _isTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
    }

    void Update()
    {
        foreach (var obj in virtualObjects)
        {
            if (obj == null) continue;
            Vector3 target = _isTracked ? visibleScale : hiddenScale;
            float speed = _isTracked ? appearSpeed : disappearSpeed;
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, target, Time.deltaTime * speed);
        }
    }
}
