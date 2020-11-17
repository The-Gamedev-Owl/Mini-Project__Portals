using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* Ground Check */
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] float _groundDistance = 0.4f;
    [SerializeField] LayerMask _groundMask;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] float _moveSpeed = 12f;
    [SerializeField] float _gravity= -9.81f;
    [SerializeField] float _jumpHeight= 3f;

    private Vector3 _currentVelocity;
    public bool _isGrounded;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundDistance, _groundMask);
        if (_isGrounded && _currentVelocity.y < 0)
            _currentVelocity.y = -3f;

        _characterController.Move(move * _moveSpeed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && _isGrounded)
            _currentVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _currentVelocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_currentVelocity * Time.deltaTime);
    }
}
