using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum Controls
    {
        WASD,
        Arrow
    }

    public Controls ControlType;
    public float BaseMovementSpeed;

    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _input.Set(Input.GetAxis($"{ControlType}_Horizontal"), Input.GetAxis($"{ControlType}_Vertical"));
    }

    private void FixedUpdate()
    {
        _rb.velocity = _input * BaseMovementSpeed;
    }
}
