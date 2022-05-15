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

    private const string TWEEN_ID_SEAM_MOVE = "TweenIDSeamMove";
    private const string TWEEN_ID_SEAM_SHAKE = "TweenIDSeamShake";

    #endregion

    #region Private Variables

    [Header("World Background")]
    [SerializeField] private RectTransform _backgroundCanvas;
    [SerializeField] private Image _leftBackground;
    [SerializeField] private Image _rightBackground;
    [SerializeField] private Image _seamGlow;
    [SerializeField] private Image _seam;
    [SerializeField] private Transform _seamCollider;

    [Header("Seam Tween Properties")]
    [SerializeField] private float _tweenTime = 0.75f;
    [SerializeField] private Ease _easeType = Ease.InOutBounce;

    [Header("Scoring Properties Data")]
    [SerializeField] private ScoringPropertiesData _scoringPropertiesData;

    [Space]
    [SerializeField] private bool _juiceItOrLoseIt;

    private PreviousTweenEndData _previousTweenEndData;

    #endregion

    public Vector3 Alpha 
    { 
        get { return new Vector3(_seamGlow.color.a, 0f, 0f); }
        set { _seamGlow.color = new Color(_seamGlow.color.r, _seamGlow.color.g, _seamGlow.color.b, value.x); }
    }

    private void Awake()
    {
        GameHelper.IsNull(_backgroundCanvas);
        GameHelper.IsNull(_leftBackground);
        GameHelper.IsNull(_rightBackground);
        GameHelper.IsNull(_seamGlow);
        GameHelper.IsNull(_seam);
        GameHelper.IsNull(_seamCollider);
        GameHelper.IsNull(_scoringPropertiesData);

        _previousTweenEndData = new PreviousTweenEndData();

        Subscribe();
    }

    private void Start()
    {
        _seam.rectTransform.DOShakeScale(3.0f, new Vector3(0.1f, 0f, 0f), 15, 20, false)
                                .SetLoops(-1)
                                .SetId(TWEEN_ID_SEAM_SHAKE);
        DOTween.Shake(() => Alpha, a => Alpha = a, 3.0f, 0.15f, 10, 15, fadeOut: false)
               .SetLoops(-1)
               .SetId(TWEEN_ID_SEAM_SHAKE);
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener<Portal.Side, int, Thing>(Events.UPDATE_SEAM_POSITION, OnPlayerScored);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<Portal.Side, int, Thing>(Events.UPDATE_SEAM_POSITION, OnPlayerScored);
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
        float movementDelta = ((float)delta / _scoringPropertiesData.MaxScoreDifference * _scoringPropertiesData.MaxScreenPercentage) / 100f;
        UpdateWorldBackground(side, movementDelta, thing);
    }

    private float GetEndValue(float currentValue, float previousEndData, float newDelta)
    {
        if (previousEndData == 0 || previousEndData == currentValue) 
        {
            return currentValue + newDelta;
        }

        return previousEndData + newDelta;
    }

    private void UpdateWorldBackground(Portal.Side side, float movementDelta, Thing thing)
    {
        DOTween.Kill(TWEEN_ID_SEAM_MOVE);

        // Calculate direction based on the side that was scored on. If scored on the left, the direction of movement
        // is positive relative to the left side. If scored on the right, the direction of movement is negative relative
        // to the left side.
        int direction = CalculateDirection(side, thing);

        // Calculate the percentage delta that the world seam and collider will move.
        float seamXDelta = _backgroundCanvas.sizeDelta.x * movementDelta * direction;
        float seamEndX = GetEndValue(_seam.rectTransform.anchoredPosition.x, _previousTweenEndData.SeamEndX, seamXDelta);

        float seamColliderXDelta = WORLD_BOUNDS.y * movementDelta * direction;
        float seamColliderEndX =
            GetEndValue(_seamCollider.transform.position.x, _previousTweenEndData.SeamColliderEndX, seamColliderXDelta);

        float leftFillDelta = movementDelta * direction;
        float leftEndFill = GetEndValue(_leftBackground.fillAmount, _previousTweenEndData.LeftEndFill, leftFillDelta);

        float rightFillDelta = movementDelta * (direction * -1);
        float rightEndFill = GetEndValue(_rightBackground.fillAmount, _previousTweenEndData.RightEndFill, rightFillDelta);

        if (_juiceItOrLoseIt)
        {
            // Cache the end position data in case the tweens are killed midway.
            _previousTweenEndData.SeamEndX = seamEndX;
            _previousTweenEndData.SeamColliderEndX = seamColliderEndX;
            _previousTweenEndData.LeftEndFill = leftEndFill;
            _previousTweenEndData.RightEndFill = rightEndFill;

            // Apply the delta in the direction calculated using tweens.
            _leftBackground.DOFillAmount(leftEndFill, _tweenTime)
                            .SetEase(_easeType)
                            .SetId(TWEEN_ID_SEAM_MOVE);
            _seam.rectTransform.DOAnchorPosX(seamEndX, _tweenTime)
                                .SetEase(_easeType)
                                .SetId(TWEEN_ID_SEAM_MOVE);
            _seamGlow.rectTransform.DOAnchorPosX(seamEndX, _tweenTime)
                                    .SetEase(_easeType)
                                    .SetId(TWEEN_ID_SEAM_MOVE);
            _seamCollider.transform.DOMoveX(seamColliderEndX, _tweenTime)
                                    .SetEase(_easeType)
                                    .SetId(TWEEN_ID_SEAM_MOVE);

            // We use the inverse direction calculated because the direction is calculated relative to the left side.
            _rightBackground.DOFillAmount(rightEndFill, _tweenTime)
                            .SetEase(_easeType)
                            .SetId(TWEEN_ID_SEAM_MOVE);
        }
        else
        {
            _leftBackground.fillAmount = leftEndFill;
            _seam.rectTransform.anchoredPosition = new Vector2(seamEndX, _seam.rectTransform.anchoredPosition.y);
            _seamGlow.rectTransform.anchoredPosition = new Vector2(seamEndX, _seamGlow.rectTransform.anchoredPosition.y);
            _seamCollider.transform.position =
                new Vector3(seamColliderEndX, _seamCollider.transform.position.y, _seamCollider.transform.position.z);
            _rightBackground.fillAmount = rightEndFill;
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(TWEEN_ID_SEAM_MOVE);
        DOTween.Kill(TWEEN_ID_SEAM_SHAKE);
        Unsubscribe();
    }

    #endregion
}
