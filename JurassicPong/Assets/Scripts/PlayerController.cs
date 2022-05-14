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
    private Vector2 _clampedPosition;

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

        Move();
    }

    private void FixedUpdate()
    {
        
    }

    private void Move()
    {
        _rb.velocity = _input * BaseMovementSpeed;
        _clampedPosition.Set(Mathf.Clamp(_rb.position.x, WorldController.WORLD_BOUNDS.x, WorldController.WORLD_BOUNDS.y),
            Mathf.Clamp(_rb.position.y, WorldController.WORLD_BOUNDS.z, WorldController.WORLD_BOUNDS.w));
        _rb.transform.position = _clampedPosition;
    }
}
