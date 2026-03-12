using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JenniferMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController _controller;
    private JenniferInputActions _input;
    private Vector2 _moveInput;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = new JenniferInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        _input.Player.Pause.performed += ctx => FindObjectOfType<PauseMenuUI>()?.TogglePause();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        ApplyGravity();
        MoveCharacter();
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        if (_moveInput.sqrMagnitude < 0.01f) return;
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir = (camForward.normalized * _moveInput.y +
                           camRight.normalized * _moveInput.x).normalized;

        _controller.Move(moveDir * walkSpeed * Time.deltaTime);

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
