using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private Camera _camera;

    [SerializeField] float _moveSpeed = 1f;

    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    void Update()
    {
        ListenForArrowKeys();
    }

    private void ListenForArrowKeys()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _camera.transform.position += Vector3.up * _moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _camera.transform.position += Vector3.right * _moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _camera.transform.position += Vector3.down * _moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _camera.transform.position += Vector3.left * _moveSpeed * Time.deltaTime;
        }
    }
}
