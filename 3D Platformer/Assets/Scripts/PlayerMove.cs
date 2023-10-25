using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    [Header("Inputs")]
    private GameInputs _gameInputs;
    private InputAction _moveInputs;
    private InputAction _jumpInputs;

    [Header("Components")]
    private Transform _cam;
    private Rigidbody _rb;
    private Animator _anim;
    [SerializeField] private GameObject _playerGFX;

    [Header("Moving")]
    [SerializeField] private float _moveSpeed = 7f;
    private Vector3 _normal;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _jumpScale = 1.5f;
    [SerializeField] private float _jumpScaleLowBorder = -7f;
    [SerializeField] private float _jumpScaleHighBorder = 0f;
    [SerializeField] private float _reqInAirTime = 0.5f;
    private float _inAirTime;

    [Header("Checking")]
    [SerializeField] private Transform _footPos;
    [SerializeField] private LayerMask _isGround;
    [SerializeField] private Transform _ledgeCheckPos;
    [SerializeField] private Transform _climbPos;
    [SerializeField] private LayerMask _isSolid;

    private void Awake()
    {
        _gameInputs = new GameInputs();
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main.transform;
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        ReadMoveInputs();
        if (!CheckGround())
        {
            if (_rb.velocity.y < -0.1f)
            {
                _anim.SetBool("Falling", true);

                if (CheckLedge())
                {
                    // LedgeClimb();
                }
            }
            else
            {
                _anim.SetBool("InAir", true);
            }
            _inAirTime += Time.deltaTime;
        }
        else
        {
            _anim.SetBool("InAir", false);
            _anim.SetBool("Falling", false);
            _inAirTime = 0f;
        }

        if (_rb.velocity.y < _jumpScaleHighBorder && _rb.velocity.y > _jumpScaleLowBorder)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * _jumpScale * Time.deltaTime;
        }
        if (_jumpInputs.WasReleasedThisFrame() && _rb.velocity.y > 0.01f)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (_inAirTime < _reqInAirTime)
            {
                _anim.SetTrigger("AirSkip");
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _normal = other.GetContact(0).normal;
    }

    private void OnEnable()
    {
        _gameInputs.Enable();
        _moveInputs = _gameInputs.Gameplay.Move;
        _jumpInputs = _gameInputs.Gameplay.Jump;
        _jumpInputs.performed += context => Jump();
    }

    private void OnDisable()
    {
        _jumpInputs.performed -= context => Jump();
        _gameInputs.Disable();
    }

    private bool CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        if (_footPos) origin = _footPos.position;
        return Physics.Raycast(origin, Vector3.down, 0.7f, _isGround);
    }

    private void ReadMoveInputs()
    {
        Vector2 inputDirection = _moveInputs.ReadValue<Vector2>();
        if (inputDirection.magnitude >= 0.1f) _anim.SetBool("IsRunning", true);
        else _anim.SetBool("IsRunning", false);
        Vector3 direction = new Vector3(inputDirection.x, 0f, inputDirection.y);
        Move(direction);
    }

    private void Move(Vector3 inputVector)
    {
        Vector3 moveDirection = _cam.forward * inputVector.z + _cam.right * inputVector.x;
        moveDirection.y = _rb.velocity.y / _moveSpeed;

        _rb.velocity = moveDirection * _moveSpeed;

        if (inputVector.magnitude > 0.01f)
        {
            _playerGFX.transform.forward = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        }
    }

    private void Jump()
    {
        if (CheckGround())
        {
            _anim.SetTrigger("Jump");
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }

    private void LedgeClimb()
    {
        Debug.Log("Ledge");
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z + 1f);
        if (_climbPos) origin = _climbPos.position;
        if (!Physics.Raycast(origin, Vector3.up, 3.1f, _isSolid))
        {
            Debug.Log("HaveSpase");
            _anim.SetTrigger("Climbing");
            _rb.useGravity = false;
        }
    }

    public void MoveToClimbPos()
    {
        transform.position = _climbPos.position;
        _rb.useGravity = true;
    }

    private bool CheckLedge()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        if (_ledgeCheckPos) origin = _ledgeCheckPos.position;
        return Physics.Raycast(origin, Vector3.forward, 0.7f, _isGround);
    }
}
