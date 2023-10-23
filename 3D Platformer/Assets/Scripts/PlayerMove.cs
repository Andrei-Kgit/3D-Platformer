using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Transform _cam;
    private GameInputs _gameInputs;
    private InputAction _move;
    private InputAction _jump;
    private Rigidbody _rb;

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _rotationStep = 0.1f;
    private float _rotationVelocity;

    private void Awake()
    {
        _gameInputs = new GameInputs();
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
    }

    void Update()
    {
        ReadMoveInputs();
    }

    private void OnEnable()
    {
        _gameInputs.Enable();
        _move = _gameInputs.Gameplay.Move;
        _jump = _gameInputs.Gameplay.Jump;

        _jump.performed += OnPlayerJump;
    }

    private void OnDisable()
    {
        _jump.performed -= OnPlayerJump;

        _gameInputs.Disable();
    }

    private void OnPlayerJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void ReadMoveInputs()
    {
        Vector2 inputDirection = _move.ReadValue<Vector2>();
        Move(inputDirection);
    }

    private void Move(Vector2 inputVector)
    {
        Vector3 Direction = new Vector3(inputVector.x, 0f, inputVector.y);
        //transform.LookAt(transform.position + moveDirection);
        if (Direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationStep);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _rb.MovePosition(transform.position + moveDirection * _moveSpeed * Time.deltaTime);
        }

    }

    private void Jump()
    {

    }
}
