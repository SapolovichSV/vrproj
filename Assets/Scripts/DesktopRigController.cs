using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopRigController : MonoBehaviour
{
    [Header("References")]
    public Transform yawRoot;
    public Transform pitchRoot;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float runMultiplier = 2f;

    [Header("Look")]
    public float lookSensitivity = 0.12f;
    public bool holdRightMouseToLook = true;
    public float minPitch = -75f;
    public float maxPitch = 75f;

    float _yaw;
    float _pitch;

    void Awake()
    {
        if (!yawRoot) yawRoot = transform;
        if (!pitchRoot) pitchRoot = Camera.main ? Camera.main.transform : transform;

        var e = yawRoot.rotation.eulerAngles;
        _yaw = e.y;
        _pitch = pitchRoot.localRotation.eulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    void Update()
    {
        if (yawRoot == null || pitchRoot == null) return;
        if (Mouse.current == null || Keyboard.current == null) return;

        var looking = !holdRightMouseToLook || Mouse.current.rightButton.isPressed;
        if (looking)
        {
            var delta = Mouse.current.delta.ReadValue();
            _yaw += delta.x * lookSensitivity;
            _pitch -= delta.y * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        yawRoot.rotation = Quaternion.Euler(0f, _yaw, 0f);
        pitchRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        var move = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) move.y += 1f;
        if (Keyboard.current.sKey.isPressed) move.y -= 1f;
        if (Keyboard.current.dKey.isPressed) move.x += 1f;
        if (Keyboard.current.aKey.isPressed) move.x -= 1f;
        if (move.sqrMagnitude > 1f) move.Normalize();

        var speed = moveSpeed;
        if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
            speed *= runMultiplier;

        var fwd = yawRoot.forward;
        fwd.y = 0f;
        fwd.Normalize();
        var right = yawRoot.right;
        right.y = 0f;
        right.Normalize();

        var deltaPos = (fwd * move.y + right * move.x) * (speed * Time.deltaTime);
        yawRoot.position += deltaPos;
    }
}

