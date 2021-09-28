﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpHeight = 3f;

    [SerializeField] private Transform _ground;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;

    byte jumps = 0;
    const byte maxJumps = 1;
    private Vector3 _velocity;
    private bool _isGrounded;
    public Slider health;

    private byte vulnerability = 8;
    void OnCollisionEnter(Collision collision)
    {
        vulnerability--;
        health.value = vulnerability;
        if (vulnerability <= 0)
        {
            Application.Quit();
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_ground.position, _groundDistance, _groundMask);
            
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        _controller.Move(move * _speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && jumps < maxJumps)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * _gravity * -2);
            jumps++;
        }
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
        Debug.Log(_isGrounded);
        if (_isGrounded)
            jumps = 0;
    }
}
