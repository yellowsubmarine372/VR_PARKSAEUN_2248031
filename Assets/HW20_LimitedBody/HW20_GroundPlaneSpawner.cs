using UnityEngine;
using Vuforia;

/// <summary>
/// GroundPlaneTarget 위에서 Tap으로 오브젝트를 소환합니다.
/// 바닥 현실 공간에 가상 오브젝트를 "착지"시키는 경험.
/// </summary>
public class HW20_GroundPlaneSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public int maxSpawnCount = 5;
    public float spawnLerpSpeed = 4f;

    private AnchorBehaviour _anchor;
    private Camera _arCamera;
    private int _spawnCount = 0;

    void Start()
    {
        _anchor = GetComponent<AnchorBehaviour>();
        _arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            TrySpawn(Input.GetTouch(0).position);
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            TrySpawn(Input.mousePosition);
#endif
    }

    void TrySpawn(Vector2 screenPos)
    {
        if (prefabToSpawn == null || _spawnCount >= maxSpawnCount) return;
        if (_arCamera == null) return;

        Ray ray = _arCamera.ScreenPointToRay(screenPos);
        // GroundPlane은 y=0 평면 기준
        Plane ground = new Plane(Vector3.up, transform.position);
        if (ground.Raycast(ray, out float dist))
        {
            Vector3 spawnPos = ray.GetPoint(dist);
            GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
            spawned.transform.localScale = Vector3.zero;
            StartCoroutine(LerpSpawn(spawned));
            _spawnCount++;
        }
    }

    System.Collections.IEnumerator LerpSpawn(GameObject obj)
    {
        Vector3 target = Vector3.one * 0.05f;
        while (obj != null && Vector3.Distance(obj.transform.localScale, target) > 0.001f)
        {
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, target, Time.deltaTime * spawnLerpSpeed);
            yield return null;
        }
        if (obj != null) obj.transform.localScale = target;
    }
}
