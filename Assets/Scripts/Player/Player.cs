using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputActionAsset InputAction;
    public CharacterController _playerCharacterController;
    
    private InputAction _playerMoveAction;
    private InputAction _playerJumpAction;
    
    [SerializeField] private Transform _cameraTransform;
    
    private Vector2 _playerMoveAmount;

    private float _playerWalkSpeed = 5.0f;
    private float _playerRotateDampening = 0.1f;
    private float _turnSmoothingVelocity;
    
    private float _verticalVelocity = 0f;
    private float _gravity = -9.8f;
    private float _jumpHeight = 5.0f;

    private void OnEnable()
    {
        InputAction.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputAction.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        _playerMoveAction = InputSystem.actions.FindAction("Move");
        _playerJumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        _playerMoveAmount = _playerMoveAction.ReadValue<Vector2>();
        PlayerMoveAndRotate();
        Jump();
    }

    private void PlayerMoveAndRotate()
    {
        Vector3 _playerDirection = new Vector3(_playerMoveAmount.x, 0f, _playerMoveAmount.y).normalized;
        Vector3 _verticalMove = new Vector3(0f, _verticalVelocity, 0f) *  Time.deltaTime;

        if (_playerDirection.magnitude >= 0.1f)
        {
            float _targetAngle = Mathf.Atan2(_playerDirection.x, _playerDirection.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle,
                ref _turnSmoothingVelocity, _playerRotateDampening);
            
            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);
            
            Vector3 moveDirection = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
            _playerCharacterController.Move(moveDirection.normalized * _playerWalkSpeed * Time.deltaTime + _verticalMove);
        }
        else
        {
            _playerCharacterController.Move(_verticalMove);
        }
    }

    private void Jump()
    {
        if (_playerCharacterController.isGrounded)
        {
            _verticalVelocity = -1f;
            if (_playerJumpAction.WasPressedThisFrame())
            {
                _verticalVelocity = _jumpHeight;
            }
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }
    
}