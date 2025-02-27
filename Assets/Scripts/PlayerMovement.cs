using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Camera Bob Settings")]
    [SerializeField] private float _bobSpeed = 10f;
    [SerializeField] private float _bobAmount = 0.05f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float _mouseSensitivity = 5f;
    [SerializeField] private float _verticalClampAngle = 85f;

    private CharacterController _controller;
    private Camera _playerCamera;
    private AudioSource _footstepAudio;
    private Vector3 _velocity;
    private bool _isCrouching = false;
    private float _originalHeight;
    private Vector3 _originalCamPos;
    private float _bobTimer;
    private float _cameraXRotation;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerCamera = Camera.main;
        _footstepAudio = GetComponent<AudioSource>();
        _originalHeight = _controller.height;
        _originalCamPos = _playerCamera.transform.localPosition;
        _cameraXRotation = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraBob();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        move = move.normalized * _moveSpeed;

        if (_controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _velocity.y = _jumpForce;
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.X))
            ToggleCrouch();

        _controller.Move((move + _velocity) * Time.deltaTime);
    }

    private void HandleCameraBob()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (moveInput.magnitude > 0 && _controller.isGrounded)
        {
            _bobTimer += Time.deltaTime * _bobSpeed;
            float bob = Mathf.Sin(_bobTimer) * _bobAmount;
            _playerCamera.transform.localPosition = _originalCamPos + new Vector3(0, bob, 0);
            if (!_footstepAudio.isPlaying) _footstepAudio.Play();
        }
        else
        {
            _playerCamera.transform.localPosition = Vector3.Lerp(
                _playerCamera.transform.localPosition,
                _originalCamPos,
                Time.deltaTime * 5f
            );
            _footstepAudio.Stop();
        }
    }

    private void HandleMouseLook()
    {
        // Получаем ввод мыши без Time.deltaTime для большей отзывчивости
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        // Поворот тела игрока по горизонтали
        transform.Rotate(Vector3.up * mouseX);

        // Поворот камеры по вертикали с ограничением
        _cameraXRotation -= mouseY;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -_verticalClampAngle, _verticalClampAngle);
        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraXRotation, 0f, 0f);
    }

    private void ToggleCrouch()
    {
        _isCrouching = !_isCrouching;
        _controller.height = _isCrouching ? _originalHeight / 2 : _originalHeight;
        Vector3 camPos = _playerCamera.transform.localPosition;
        camPos.y = _isCrouching ? _originalCamPos.y / 2 : _originalCamPos.y;
        _playerCamera.transform.localPosition = camPos;
    }
}