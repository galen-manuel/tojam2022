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
    [SerializeField] private Image _leftWorldBackground;
    [SerializeField] private Image _rightWorldBackground;
    [SerializeField] private Image _worldSeamGlow;
    [SerializeField] private Image _worldSeam;
    [SerializeField] private Transform _worldSeamCollider;

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
        get { return new Vector3(_worldSeamGlow.color.a, 0f, 0f); }
        set 
        { 
            
            _worldSeamGlow.color = new Color(_worldSeamGlow.color.r, 
                                             _worldSeamGlow.color.g, 
                                             _worldSeamGlow.color.b, 
                                             value.x);  
        }
    }

    private void Awake()
    {
        GameHelper.IsNull(_backgroundCanvas);
        GameHelper.IsNull(_leftWorldBackground);
        GameHelper.IsNull(_rightWorldBackground);
        GameHelper.IsNull(_worldSeamGlow);
        GameHelper.IsNull(_worldSeam);
        GameHelper.IsNull(_worldSeamCollider);
        GameHelper.IsNull(_scoringPropertiesData);

        Subscribe();
    }

    private void Start()
    {
        _worldSeam.rectTransform.DOShakeScale(3.0f, new Vector3(0.1f, 0f, 0f), 15, 20, false)
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

    private struct PreviousTweenEndData
    {
        public float WorldSeamEndXValue;
        public float WorldSeamColliderEndXValue;
        public float LeftEndFillValue;
        public float RightEndFillValue;
    }

    private void UpdateWorldBackground(Portal.Side side, float movementDelta, Thing thing)
    {
        DOTween.Kill(TWEEN_ID_SEAM_MOVE);

        // Calculate direction based on the side that was scored on. If scored on the left, the direction of movement
        // is positive relative to the left side. If scored on the right, the direction of movement is negative relative
        // to the left side.
        int direction = CalculateDirection(side, thing);

        // Calculate the percentage delta that the world seam and collider will move.
        //_previousTweenEndData = new PreviousTweenEndData()
        //{

        //};
        float worldSeamXDelta = _backgroundCanvas.sizeDelta.x * movementDelta * direction;
        float worldSeamEndXValue = _worldSeam.rectTransform.anchoredPosition.x + worldSeamXDelta;

        float worldSeamColliderXDelta = WORLD_BOUNDS.y * movementDelta * direction;
        float worldSeamColliderEndXValue = _worldSeamCollider.transform.position.x + worldSeamColliderXDelta;

        float leftFillDelta = movementDelta * direction;
        float leftEndFill = _leftWorldBackground.fillAmount + leftFillDelta;

        float rightFillDelta = movementDelta * (direction * -1);
        float rightEndFill = _rightWorldBackground.fillAmount + rightFillDelta;

        if (_juiceItOrLoseIt)
        {
            // Apply the delta in the direction calculated using tweens.
            _leftWorldBackground.DOFillAmount(leftEndFill, _tweenTime)
                                .SetEase(_easeType)
                                .SetId(TWEEN_ID_SEAM_MOVE);
            _worldSeam.rectTransform.DOAnchorPosX(worldSeamEndXValue, _tweenTime)
                                    .SetEase(_easeType)
                                    .SetId(TWEEN_ID_SEAM_MOVE);
            _worldSeamGlow.rectTransform.DOAnchorPosX(worldSeamEndXValue, _tweenTime)
                                    .SetEase(_easeType)
                                    .SetId(TWEEN_ID_SEAM_MOVE);
            _worldSeamCollider.transform.DOMoveX(worldSeamColliderEndXValue, _tweenTime)
                                        .SetEase(_easeType)
                                        .SetId(TWEEN_ID_SEAM_MOVE);

            // We use the inverse direction calculated because the direction is calculated relative to the left side.
            _rightWorldBackground.DOFillAmount(rightEndFill, _tweenTime)
                                 .SetEase(_easeType)
                                 .SetId(TWEEN_ID_SEAM_MOVE);
        }
        else
        {
            _leftWorldBackground.fillAmount = leftEndFill;
            _worldSeam.rectTransform.anchoredPosition = new Vector2(worldSeamEndXValue, 
                                                                    _worldSeam.rectTransform.anchoredPosition.y);
            _worldSeamGlow.rectTransform.anchoredPosition = new Vector2(worldSeamEndXValue,
                                                                        _worldSeamGlow.rectTransform.anchoredPosition.y);
            _worldSeamCollider.transform.position = new Vector3(worldSeamColliderEndXValue,
                                                                _worldSeamCollider.transform.position.y,
                                                                _worldSeamCollider.transform.position.z);
            _rightWorldBackground.fillAmount = rightEndFill;
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
