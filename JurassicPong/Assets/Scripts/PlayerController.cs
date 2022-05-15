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

    [Header("Character Properties")]
    public float BaseMovementSpeed;
    public float Width;
    public float Height;

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
}
