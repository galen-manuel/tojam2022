using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Thing : MonoBehaviour
{
    public float VelocityMultiplier;

    protected Rigidbody2D _rb;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
