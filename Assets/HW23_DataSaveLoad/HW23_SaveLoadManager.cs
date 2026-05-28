using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// WorldData / TransformData / SaveableEntity 는 Player_Demo_SaveData 폴더의 기존 클래스 재사용
public class HW23_SaveLoadManager : MonoBehaviour
{
    public static HW23_SaveLoadManager Instance { get; private set; }

    [SerializeField] string saveFileName = "HW23_worldData";

    string _status = "준비 완료 — F5:저장 / F9:불러오기 / Del:초기화 / Ctrl+R:씬 재시작";
    string SavePath => Path.Combine(Application.persistentDataPath, saveFileName + ".json");
    Texture2D _bgTex;
    bool _guiLogged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(LoadAfterFrame());
    }

    IEnumerator LoadAfterFrame()
    {
        yield return null;
        LoadData();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.f5Key.wasPressedThisFrame) SaveData();
        if (Keyboard.current.f9Key.wasPressedThisFrame) LoadData();
        if (Keyboard.current.deleteKey.wasPressedThisFrame) ResetData();
        if (Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
            RestartScene();
    }

    public void SaveData()
    {
        var targets = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
        if (targets.Length == 0)
        {
            _status = "[저장 실패] SaveableEntity 오브젝트가 없음";
            return;
        }

        WorldData data = new WorldData();
        foreach (var entity in targets)
        {
            if (entity == null) continue;
            data.objects.Add(new TransformData
            {
                objectName = entity.gameObject.name,
                position   = entity.transform.position,
                rotation   = entity.transform.rotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        _status = $"저장 완료 ({data.objects.Count}개) | {System.DateTime.Now:HH:mm:ss}";
        Debug.Log($"[HW23] 저장 완료: {SavePath}");
    }

    public void LoadData()
    {
        if (!File.Exists(SavePath))
        {
            _status = "저장 파일 없음 — 기본 위치 유지";
            Debug.Log("[HW23] 저장 파일 없음.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        WorldData data = JsonUtility.FromJson<WorldData>(json);
        if (data == null || data.objects == null) return;

        var savedMap = new Dictionary<string, TransformData>();
        foreach (var t in data.objects)
            if (!savedMap.ContainsKey(t.objectName))
                savedMap[t.objectName] = t;

        var targets = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
        foreach (var entity in targets)
        {
            if (entity == null) continue;
            if (!savedMap.TryGetValue(entity.gameObject.name, out TransformData td)) continue;

            entity.transform.position = td.position;
            entity.transform.rotation = td.rotation;

            if (entity.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity        = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if (entity.TryGetComponent<HW23_PlayerController>(out var pc))
                pc.ResetVelocity();
        }

        _status = $"불러오기 완료 ({data.objects.Count}개) | {System.DateTime.Now:HH:mm:ss}";
        Debug.Log("[HW23] 불러오기 완료.");
    }

    public void ResetData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            _status = "데이터 초기화 완료 (파일 삭제됨)";
            Debug.Log("[HW23] 저장 데이터 삭제됨.");
        }
        else
        {
            _status = "삭제할 파일 없음";
        }
    }

    public void RestartScene()
    {
        SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationFocus(bool hasFocus)  { if (!hasFocus) SaveData(); }
    void OnApplicationPause(bool isPaused)  { if (isPaused)  SaveData(); }
    void OnApplicationQuit()                { SaveData(); }

    // ──────────────── UI ────────────────
    void OnGUI()
    {
        // 진단 로그 (1회만)
        if (!_guiLogged)
        {
            Debug.Log($"[HW23] OnGUI 호출 확인. 오브젝트 선택: {HW23_ObjectMover.GetSelectedName()}");
            _guiLogged = true;
        }

        // 불투명 배경 박스
        if (_bgTex == null)
        {
            _bgTex = new Texture2D(1, 1);
            _bgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.75f));
            _bgTex.Apply();
        }
        var bgStyle = new GUIStyle();
        bgStyle.normal.background = _bgTex;
        GUI.Box(new Rect(5, 5, 340, 320), GUIContent.none, bgStyle);

        // 흰색 텍스트 스타일
        var bold  = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        bold.normal.textColor = Color.white;

        var small = new GUIStyle(GUI.skin.label) { fontSize = 12 };
        small.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

        var btn  = new GUIStyle(GUI.skin.button) { fontSize = 13 };
        var stat = new GUIStyle(GUI.skin.label)  { fontSize = 12, wordWrap = true };
        stat.normal.textColor = Color.yellow;

        GUI.Label(new Rect(10, 10, 325, 24), "=== HW23: 플레이어 상태 저장 시스템 ===", bold);

        GUI.Label(new Rect(10,  38, 325, 20), "[ 플레이어 ]", bold);
        GUI.Label(new Rect(10,  58, 325, 18), "WASD: 이동  |  Mouse: 시점  |  Space: 점프", small);
        GUI.Label(new Rect(10,  76, 325, 18), "ESC: 마우스 잠금 해제 (버튼 클릭)", small);

        GUI.Label(new Rect(10, 100, 325, 20), "[ 오브젝트 이동 ]", bold);
        GUI.Label(new Rect(10, 120, 325, 18), "N: 오브젝트 선택 전환", small);
        GUI.Label(new Rect(10, 138, 325, 18), "IJKL: XZ 이동  |  PageUp/Down: Y 이동", small);

        var selStyle = new GUIStyle(small);
        selStyle.normal.textColor = Color.cyan;
        GUI.Label(new Rect(10, 156, 325, 18), $"▶ 선택: {HW23_ObjectMover.GetSelectedName()}", selStyle);

        GUI.Label(new Rect(10, 180, 325, 20), "[ 저장 / 불러오기 ]", bold);
        GUI.Label(new Rect(10, 200, 325, 18), "F5: 저장  |  F9: 불러오기  |  Del: 초기화", small);
        GUI.Label(new Rect(10, 218, 325, 18), "Ctrl+R: 씬 재시작 (저장 후 재로드)", small);

        if (GUI.Button(new Rect(10,  244, 95, 32), "저장 (F5)",    btn)) SaveData();
        if (GUI.Button(new Rect(115, 244, 95, 32), "불러오기 (F9)", btn)) LoadData();
        if (GUI.Button(new Rect(220, 244, 105, 32), "초기화 (Del)", btn)) ResetData();

        GUI.Label(new Rect(10, 282, 325, 35), _status, stat);
    }
}
