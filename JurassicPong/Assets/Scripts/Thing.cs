using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Thing : MonoBehaviour
{
    public float VelocityMultiplier;

    [Header("Spawn In")]
    public Ease SpawnInEase = Ease.InSine;
    public float SpawnInTime = 0.25f;
    [Space()]
    [Header("Spawn Out")]
    public Ease SpawnOutEase = Ease.OutSine;
    public float SpawnOutTime = 0.25f;

    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;

    public Vector2 Velocity {
        get { return _rb.velocity; }
        set { _rb.velocity = value; }
    }

    public float AngularVelocity 
    {
        get { return _rb.angularVelocity;  }
        set { _rb.angularVelocity = value; }
    }

    public virtual void Init(Vector2 startingVelocity = default, SpriteRenderer spriteRenderer = null)
    {
        Vector2 scale = transform.localScale;
        transform.localScale = Vector2.zero;
        if (startingVelocity == default)
        {
            startingVelocity = Random.onUnitSphere;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer = spriteRenderer;
        _rb.velocity = startingVelocity * VelocityMultiplier;
        SpawnIn(scale);
    }

    public void SpawnOut()
    {
        _spriteRenderer.DOFade(0, SpawnOutTime * 0.9f);
        transform.DOScale(0.1f, SpawnOutTime).SetEase(SpawnOutEase);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void SpawnIn(Vector2 scale)
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        _spriteRenderer.DOFade(1, SpawnInTime);
        transform.DOScale(scale.x, SpawnInTime).SetEase(SpawnInEase);
    }
}
