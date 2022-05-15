using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    #endregion

    #region Private Variables

    private Rigidbody2D _rb;
    private Vector2 _input;
    private Vector2 _clampedPosition;

    private bool _isGameOver;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Subscribe();
    }

    private void Update()
    {
        if (_isGameOver)
        {
            return;
        }

        _input.Set(Input.GetAxis($"{ControlType}_Horizontal"), Input.GetAxis($"{ControlType}_Vertical"));

        Move();
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener(Events.GAME_OVER, OnGameOver);
    }

    private void Unsubscribe()
    {
        Messenger.AddListener(Events.GAME_OVER, OnGameOver);
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

    private void OnGameOver()
    {
        _isGameOver = true;
        _rb.velocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
