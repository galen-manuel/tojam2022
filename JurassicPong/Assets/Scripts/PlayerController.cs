using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public enum Controls
    {
        WASD,
        Arrow
    }

    #region Public Variables

    public Controls ControlType;

    [Header("Character Properties")]
    public float BaseMovementSpeed;
    public float Width;
    public float Height;

    [Tooltip("The colour the player flashes when respawning.")]
    public Color DeathFlashColour = Color.red;
    [Tooltip("The number of times the player flashes on respawn.")]
    public int DeathLoopCount = 5;
    [Tooltip("The time for each loop on respawn.")]
    public float DeathLoopTime = 0.25f;

    #endregion

    #region Private Variables

    private Rigidbody2D _rb;
    private SpriteRenderer _mainRenderer;
    private Vector2 _input;
    private Vector2 _clampedPosition;
    private Vector2 _respawnPosition;

    private bool _isPlayable;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainRenderer = GetComponent<SpriteRenderer>();
        Subscribe();
    }

    private void Start()
    {
        _respawnPosition = transform.position;
    }

    private void Update()
    {
        if (!_isPlayable)
        {
            return;
        }

        _input.Set(Input.GetAxis($"{ControlType}_Horizontal"), Input.GetAxis($"{ControlType}_Vertical"));

        Move();
    }

    #region Public Methods

    public void Respawn()
    {
        _isPlayable = false;
        _rb.velocity = Vector2.zero;
        _input = Vector2.zero;
        _rb.MovePosition(_respawnPosition);
        _mainRenderer.DOColor(DeathFlashColour, DeathLoopTime).SetLoops(DeathLoopCount, LoopType.Yoyo).OnComplete(() =>
        {
            _mainRenderer.color = Color.white;
            _isPlayable = true;
        });
    }

    #endregion

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener<int>(Events.START_GAME, OnStartGame);
        Messenger.AddListener(Events.GAME_OVER, OnGameOver);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<int>(Events.START_GAME, OnStartGame);
        Messenger.RemoveListener(Events.GAME_OVER, OnGameOver);
    }

    private void Move()
    {
        _rb.velocity = _input * BaseMovementSpeed;

        // Calculate world boundaries taking into account the width and height of each character.
        float worldXMin = WorldController.WORLD_BOUNDS.x + Width * 0.5f;
        float worldXMax = WorldController.WORLD_BOUNDS.y - Width * 0.5f;
        float worldYMin = WorldController.WORLD_BOUNDS.z + Height * 0.5f;
        float worldYMax = WorldController.WORLD_BOUNDS.w - Height * 0.5f;

        // Figure out player's X and Y clamp values.
        float xClamp = Mathf.Clamp(_rb.position.x, worldXMin, worldXMax);
        float yClamp = Mathf.Clamp(_rb.position.y, worldYMin, worldYMax);
       
        // Clamp the player.
        _clampedPosition.Set(xClamp, yClamp);
        _rb.transform.position = _clampedPosition;
    }

    #endregion

    #region Event Handlers
    private void OnStartGame(int startingGameTime)
    {
        _isPlayable = true;
    }

    private void OnGameOver()
    {
        _isPlayable = false;
        _rb.velocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
