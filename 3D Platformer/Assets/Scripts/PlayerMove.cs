using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    private Transform _cam;
    private GameInputs _gameInputs;
    private InputAction _move;
    private InputAction _jump;
    private Rigidbody _rb;
    private Animator _anim;
    [SerializeField] private GameObject _playerGFX;

    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpForce = 10f;

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
        }
        else
        {
            _anim.SetBool("InAir", false);
            _anim.SetBool("Falling", false);
        }    
    }

    private void OnEnable()
    {
        _gameInputs.Enable();
        _move = _gameInputs.Gameplay.Move;
        _jump = _gameInputs.Gameplay.Jump;
        _jump.performed += context => Jump();
    }

    private void OnDisable()
    {
        _jump.performed -= context => Jump();
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
        Vector2 inputDirection = _move.ReadValue<Vector2>();
        if (inputDirection.magnitude >= 0.1f) _anim.SetBool("IsRunning", true);
        else _anim.SetBool("IsRunning", false);
        Vector3 direction = new Vector3(inputDirection.x, 0f, inputDirection.y);
        Move(direction);
    }

    private void Move(Vector3 inputVector)
    {
        Vector3 moveDirection = _cam.forward * inputVector.z + _cam.right * inputVector.x;
        moveDirection.y = _rb.velocity.y / _moveSpeed;
        //Vector3 moveVelocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);
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
