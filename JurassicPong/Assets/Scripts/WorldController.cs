using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorldController : MonoBehaviour
{
    /// <summary>
    /// X = Left X Position
    /// Y = Right X Position
    /// W = Bottom Y Position
    /// Z = Top Y Position
    /// </summary>
    public static readonly Vector4 WORLD_BOUNDS = new Vector4(0f, 17.78371f, 0f, 10f);

    #region Private Variables

    [Header("Misc")]
    [SerializeField] private GameManager _gameManager;

    [Header("World Background")]
    [SerializeField] private RectTransform _backgroundCanvas;
    [SerializeField] private Image _leftWorldBackground;
    [SerializeField] private Image _rightWorldBackground;
    [SerializeField] private Image _worldSeam;
    [SerializeField] private Transform _worldSeamCollider;

    [Header("Seam Tween Properties")]
    [SerializeField] private float _tweenTime = 0.75f;
    [SerializeField] private Ease _easeType = Ease.InOutBounce;

    [Header("Scoring Properties")]
    [SerializeField] private int _maxScoreDifference = 10;
    [SerializeField] private float _maxScreenPrecentage = 25f;
    private float _percentagePerPoint;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_gameManager);
        GameHelper.IsNull(_backgroundCanvas);
        GameHelper.IsNull(_leftWorldBackground);
        GameHelper.IsNull(_rightWorldBackground);
        GameHelper.IsNull(_worldSeam);
        GameHelper.IsNull(_worldSeamCollider);

        Unsubscribe();
        Subscribe();
    }

    private void Update()
    {
        _percentagePerPoint = (1f / _maxScoreDifference * _maxScreenPrecentage) / 100f;
    }

    #region Private Methods

    private void Subscribe()
    {
        _gameManager.Scored += OnPlayerScored;
    }

    private void Unsubscribe()
    {
        _gameManager.Scored -= OnPlayerScored;
    }

    #endregion

    #region Event Handlers

    private void OnPlayerScored(Portal.Side side, int delta, Thing thing)
    {
        float movementDelta = ((float)delta / _maxScoreDifference * _maxScreenPrecentage) / 100f;
        UpdateWorldBackground(side, movementDelta, thing);
    }

    private int CalculateDirection(Portal.Side side, Thing thing)
    {
        return side switch
        {
            Portal.Side.Left => thing is BadThing ? -1 : 1,
            Portal.Side.Right => thing is BadThing ? 1 : -1,
            _ => throw new ArgumentOutOfRangeException(nameof(side)),
        };
    }

    private void UpdateWorldBackground(Portal.Side side, float movementDelta, Thing thing)
    {
        // Calculate direction based on the side that was scored on. If scored on the left, the direction of movement
        // is positive relative to the left side. If scored on the right, the direction of movement is negative relative
        // to the left side.
        int direction = CalculateDirection(side, thing);

        // Calculate the percentage delta that the world seam and collider will move.
        float worldSeamDeltaX = _backgroundCanvas.sizeDelta.x * movementDelta;
        float worldSeamColliderDeltaX = WORLD_BOUNDS.y * movementDelta;

        // Apply the delta in the direction calculated using tweens.
        float endFillValue = _leftWorldBackground.fillAmount + (movementDelta * direction);
        _leftWorldBackground.DOFillAmount(endFillValue, _tweenTime)
                            .SetEase(_easeType);
        _worldSeam.rectTransform.DOAnchorPosX(worldSeamDeltaX * direction, _tweenTime)
                                .SetRelative()
                                .SetEase(_easeType);
        _worldSeamCollider.transform.DOMoveX(worldSeamColliderDeltaX * direction, _tweenTime)
                                    .SetRelative()
                                    .SetEase(_easeType);

        // We use the inverse direction calculated because the direction is calculated relative to the left side.
        endFillValue = _rightWorldBackground.fillAmount + (movementDelta * (direction * -1));
        _rightWorldBackground.DOFillAmount(endFillValue, _tweenTime)
                             .SetEase(_easeType);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
