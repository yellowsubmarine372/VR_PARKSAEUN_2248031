using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HW23_PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.15f;

    CharacterController _controller;
    Vector3 _velocity;
    float _xRotation;
    bool _cursorLocked = true;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        SetCursorLock(true);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLock(!_cursorLocked);

        if (_cursorLocked)
            HandleMovement();
    }

    void HandleMovement()
    {
        if (_controller.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            _xRotation -= delta.y * mouseSensitivity;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * delta.x * mouseSensitivity);
        }

        if (Keyboard.current != null)
        {
            float x = 0f, z = 0f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;

            float speed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : moveSpeed;
            Vector3 move = transform.right * x + transform.forward * z;
            if (move.magnitude > 1f) move.Normalize();
            _controller.Move(move * speed * Time.deltaTime);

            if (Keyboard.current.spaceKey.wasPressedThisFrame && _controller.isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void ResetVelocity()
    {
        _velocity = Vector3.zero;
    }

    void SetCursorLock(bool locked)
    {
        _cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnDisable()
    {
        SetCursorLock(false);
    }
}
