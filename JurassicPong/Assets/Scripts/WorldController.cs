using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorldController : MonoBehaviour
{
    #region Constants

    /// <summary>
    /// X = Left X Position
    /// Y = Right X Position
    /// W = Bottom Y Position
    /// Z = Top Y Position
    /// </summary>
    public static readonly Vector4 WORLD_BOUNDS = new Vector4(0f, 17.78371f, 0f, 10f);

    private const string TWEEN_ID_SEAM_SHAKE = "TweenIDSeamShake";

    #endregion

    #region Private Variables

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

    // Debug value for display in Debug Inspector.
    private float _percentagePerPoint;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_backgroundCanvas);
        GameHelper.IsNull(_leftWorldBackground);
        GameHelper.IsNull(_rightWorldBackground);
        GameHelper.IsNull(_worldSeam);
        GameHelper.IsNull(_worldSeamCollider);

        Subscribe();
    }

    private void Update()
    {
        _percentagePerPoint = (1f / _maxScoreDifference * _maxScreenPrecentage) / 100f;
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener<Portal.Side, int, Thing>(Constants.EVENT_UPDATE_SEAM_POSITION, OnPlayerScored);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<Portal.Side, int, Thing>(Constants.EVENT_UPDATE_SEAM_POSITION, OnPlayerScored);
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

    #endregion

    #region Event Handlers

    private void OnPlayerScored(Portal.Side side, int delta, Thing thing)
    {
        float movementDelta = ((float)delta / _maxScoreDifference * _maxScreenPrecentage) / 100f;
        UpdateWorldBackground(side, movementDelta, thing);
    }

    private void UpdateWorldBackground(Portal.Side side, float movementDelta, Thing thing)
    {
        DOTween.Kill(TWEEN_ID_SEAM_SHAKE);

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
                            .SetEase(_easeType)
                            .SetId(TWEEN_ID_SEAM_SHAKE);
        _worldSeam.rectTransform.DOAnchorPosX(worldSeamDeltaX * direction, _tweenTime)
                                .SetRelative()
                                .SetEase(_easeType)
                                .SetId(TWEEN_ID_SEAM_SHAKE);
        _worldSeamCollider.transform.DOMoveX(worldSeamColliderDeltaX * direction, _tweenTime)
                                    .SetRelative()
                                    .SetEase(_easeType)
                                    .SetId(TWEEN_ID_SEAM_SHAKE);

        // We use the inverse direction calculated because the direction is calculated relative to the left side.
        endFillValue = _rightWorldBackground.fillAmount + (movementDelta * (direction * -1));
        _rightWorldBackground.DOFillAmount(endFillValue, _tweenTime)
                             .SetEase(_easeType)
                             .SetId(TWEEN_ID_SEAM_SHAKE);
    }

    private void OnDestroy()
    {
        DOTween.Kill(TWEEN_ID_SEAM_SHAKE);
        Unsubscribe();
    }

    #endregion
}
