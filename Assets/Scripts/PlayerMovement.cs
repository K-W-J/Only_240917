using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class PlayerMovement : MonoBehaviour
{
    [Header("CameraShake")]
    [Space(5f)]

    [SerializeField] private CinemachineVirtualCamera _playerVirtualCamera;
    [SerializeField] private NoiseSettings _idleShake, _walkShake, _runShake, _fallShake;

    [Space(6f)]
    [Header("PlayerValue")]
    [Space(5f)]

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _jumpPower;

    [Space(6f)]
    [Header("GroundCheck")]
    [Space(5f)]

    [SerializeField] private Mesh _groundChenkMesh;
    [SerializeField] private Transform _groundChenk;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Vector3 _boxSize;

    private CinemachineBasicMultiChannelPerlin _camNoise;
    private Camera _playerCamera;

    private Rigidbody _rb;

    private Vector3 _startPostion;

    private float _moveSpeedSafe;
    private float _smoothShakeSafe;
    private float _amplitudeGain;

    private bool _isGorund;

    private float moveCamX = 0f;
    private float moveCamY = 0f;

    private void Awake()
    {
        _camNoise = _playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _rb = GetComponent<Rigidbody>();
        _playerCamera = Camera.main;


        Cursor.lockState = CursorLockMode.Locked;

        _moveSpeedSafe = _moveSpeed;

    }
    private void Update()
    {
        CameraMovement();
        //CameraShake();

        Movement();
        Run();

        Jump();
        //isGround();
        
    }

    private void FixedUpdate()
    {
        isGround();
    }

    private void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX * _moveSpeed, _rb.velocity.y, moveY * _moveSpeed);

        moveDir = Vector3.Lerp(_startPostion, moveDir, Time.deltaTime * 10);

        moveDir.y = _rb.velocity.y;

        _rb.velocity = transform.TransformDirection(moveDir);

        _startPostion = moveDir;

    }

    private void Run()
    {

        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (_moveSpeed < _maxMoveSpeed)
                {
                    _moveSpeed += Time.deltaTime * 6;
                }
            }
            else
            {
                if (_moveSpeedSafe < _moveSpeed)
                {
                    _moveSpeed -= Time.deltaTime * 6;
                }
            }
        }
        else
        {
            _moveSpeed = _moveSpeedSafe;
        }
    }

    private void isGround()
    {
        Collider[] _gorund = Physics.OverlapBox(_groundChenk.position, _boxSize, new Quaternion(0, 0, 0, 1), _layerMask);
        _isGorund = _gorund.Length > 0;
        print(_gorund.Length);
    }

    private void OnDrawGizmos()
    {
        if (_groundChenk == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(_groundChenkMesh, 0, _groundChenk.position, new Quaternion(0, 0, 0, 1), _boxSize);
        Gizmos.color = Color.green;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGorund)
        {
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        }
    }

    private void CameraMovement()
    {

        transform.eulerAngles = new Vector3(0, _playerCamera.transform.eulerAngles.y, 0);

    }

    private void CameraShake()
    {

        if(Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            _camNoise.m_NoiseProfile = _walkShake;
            _amplitudeGain = 1;

        }
        else if(Input.GetButton("Vertical") || Input.GetButton("Horizontal") | Input.GetKey(KeyCode.LeftShift))
        {
            _camNoise.m_NoiseProfile = _runShake;
            _amplitudeGain = 2;
        }
        else
        {
            _camNoise.m_NoiseProfile = _idleShake;
            _amplitudeGain = 0.5f;
        }

    }
}
