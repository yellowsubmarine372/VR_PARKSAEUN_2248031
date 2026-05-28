using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Tab 키로 오브젝트 순환 선택, IJKL + PageUp/Down 으로 이동
public class HW23_ObjectMover : MonoBehaviour
{
    static HW23_ObjectMover _selected;
    static readonly List<HW23_ObjectMover> _all = new List<HW23_ObjectMover>();
    static int _lastCycleFrame = -1;  // 프레임당 1회만 순환 허용

    public float moveSpeed = 4f;

    bool IsSelected => _selected == this;

    void Awake()
    {
        _all.Add(this);
        if (_selected == null) _selected = this;
        Debug.Log($"[ObjectMover] {gameObject.name} 등록됨. 현재 총 {_all.Count}개");
    }

    void OnDestroy()
    {
        _all.Remove(this);
        if (_selected == this)
            _selected = _all.Count > 0 ? _all[0] : null;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.nKey.wasPressedThisFrame)
            CycleSelection();

        if (!IsSelected) return;
        MoveObject();
    }

    void CycleSelection()
    {
        // 같은 프레임에 여러 인스턴스가 중복 호출하는 것을 방지
        if (Time.frameCount == _lastCycleFrame) return;
        _lastCycleFrame = Time.frameCount;

        if (_all.Count == 0) { Debug.Log("[ObjectMover] _all 비어있음!"); return; }
        int idx = _selected != null ? _all.IndexOf(_selected) : -1;
        if (idx == -1) idx = 0;
        _selected = _all[(idx + 1) % _all.Count];
        Debug.Log($"[ObjectMover] 선택 전환 → {_selected?.name}");
    }

    void MoveObject()
    {
        float x = 0f, y = 0f, z = 0f;
        if (Keyboard.current.iKey.isPressed) z += 1f;
        if (Keyboard.current.kKey.isPressed) z -= 1f;
        if (Keyboard.current.jKey.isPressed) x -= 1f;
        if (Keyboard.current.lKey.isPressed) x += 1f;
        if (Keyboard.current.pageUpKey.isPressed) y += 1f;
        if (Keyboard.current.pageDownKey.isPressed) y -= 1f;

        transform.Translate(new Vector3(x, y, z) * moveSpeed * Time.deltaTime, Space.World);
    }

    public static string GetSelectedName()
    {
        return _selected != null ? _selected.gameObject.name : "없음";
    }
}
