using UnityEngine;
using UnityEngine.InputSystem;

public class HW21_CameraController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 60f;

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector3 move = Vector3.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    move += Vector3.forward;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  move += Vector3.back;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  move += Vector3.left;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) move += Vector3.right;
        if (keyboard.qKey.isPressed) move += Vector3.down;
        if (keyboard.eKey.isPressed) move += Vector3.up;

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        // 마우스 오른쪽 버튼 누른 채 드래그 → 카메라 회전
        var mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            transform.Rotate(Vector3.up,    delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -delta.y * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
