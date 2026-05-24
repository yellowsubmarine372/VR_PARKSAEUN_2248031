using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GroundPlaneStage 위에서 빈 공간 Tap으로 구를 소환합니다.
/// 기존 오브젝트를 클릭한 경우에는 소환하지 않습니다.
/// </summary>
public class HW20_GroundPlaneSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public int maxSpawnCount = 5;
    public float spawnLerpSpeed = 4f;
    public float spawnRadius = 0.15f; // Stage 중심에서 랜덤 배치 반경

    private Camera _arCamera;
    private int _spawnCount = 0;
    private Vector3 _prefabScale;

    void Start()
    {
        _arCamera = Camera.main;
        // 프리팹의 원래 스케일 기억
        if (prefabToSpawn != null)
            _prefabScale = prefabToSpawn.transform.localScale;
        else
            _prefabScale = Vector3.one * 0.05f;
    }

    void Update()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
            TrySpawn(pointer.position.ReadValue());
    }

    void TrySpawn(Vector2 screenPos)
    {
        if (prefabToSpawn == null || _spawnCount >= maxSpawnCount) return;
        if (_arCamera == null) return;

        // 기존 콜라이더를 탭한 경우 소환하지 않음 (큐브 탭과 충돌 방지)
        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out _)) return;

        // Stage 중심 기준으로 랜덤 위치에 소환
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0f,
            Random.Range(-spawnRadius, spawnRadius)
        );
        Vector3 spawnPos = transform.position + randomOffset;

        GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
        spawned.transform.localScale = Vector3.zero;
        StartCoroutine(LerpSpawn(spawned));
        _spawnCount++;
    }

    System.Collections.IEnumerator LerpSpawn(GameObject obj)
    {
        Vector3 target = _prefabScale;
        while (obj != null && Vector3.Distance(obj.transform.localScale, target) > 0.001f)
        {
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, target, Time.deltaTime * spawnLerpSpeed);
            yield return null;
        }
        if (obj != null) obj.transform.localScale = target;
    }
}
