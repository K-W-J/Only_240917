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
    [SerializeField] private NoiseSettings _walkShake, _runShake, _fallShake;

    [Space(6f)]
    [Header("PlayerValue")]
    [Space(5f)]

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _jumpPower;

    private CinemachineBasicMultiChannelPerlin _playerVirtualCameraShake;
    private Camera _playerCamera;

    private Rigidbody _rb;

    private Vector3 _startPostion;

    private float _moveSpeedSafe;

    private float moveCamX = 0f;
    private float moveCamY = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerCamera = Camera.main;
        _playerVirtualCameraShake = _playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Cursor.lockState = CursorLockMode.Locked;

        _moveSpeedSafe = _moveSpeed;
    }
    private void Update()
    {
        CameraMovement();
        Movement();
        Jump();
        Run();
    }

    private void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX * _moveSpeed, _rb.velocity.y, moveY * _moveSpeed);

        moveDir = Vector3.Lerp(_startPostion, moveDir, Time.deltaTime * 10);

        _rb.velocity = transform.TransformDirection(moveDir);

        _startPostion = moveDir;

    }

    private void Run()
    {
        print(_moveSpeedSafe);
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

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        _playerVirtualCameraShake.m_NoiseProfile = _walkShake;
    }
}
