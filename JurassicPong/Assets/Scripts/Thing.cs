using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Thing : MonoBehaviour
{
    public float VelocityMultiplier;

    protected Rigidbody2D _rb;

    public Vector2 Velocity {
        get { return _rb.velocity; }
        set { _rb.velocity = value; }
    }

    public float AngularVelocity 
    {
        get { return _rb.angularVelocity;  }
        set { _rb.angularVelocity = value; }
    }

    public virtual void Init(Vector2 startingVelocity = default)
    {
        if (startingVelocity == default)
        {
            startingVelocity = Random.onUnitSphere;
        }

        _rb.velocity = startingVelocity * VelocityMultiplier;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
}
