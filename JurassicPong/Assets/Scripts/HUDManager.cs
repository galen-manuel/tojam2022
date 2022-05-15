using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class HUDManager : MonoBehaviour
{
    #region Constants

    private const float TIMER_VISIBLE_Y_POSITION = 2f;
    private const string TWEEN_ID_TIMER_SCALE_PULSE = "timerScalePulse";

    #endregion

    #region Private Variables

    [Header("Misc")]
    [SerializeField] private GameManager _gameManager;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform _gameTimer;
    [SerializeField] private TextMeshProUGUI _gameTimerText;

    private float _timerHiddenYPosition;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_gameManager);
        GameHelper.IsNull(_gameTimer);
        GameHelper.IsNull(_gameTimerText);

        _timerHiddenYPosition = _gameTimer.rect.height;

        Unsubscribe();
        Subscribe();
    }

    private void Start()
    {
        ToggleHUD(true);
    }

    #region Private Methods

    private void Subscribe()
    {
        _gameManager.GameTimeTick += OnGameTimeTick;
        _gameManager.ScoreMulitplierActivated += OnScoreMultiplierActivated;
        _gameManager.GameEnded += OnGameEnded;
    }

    private void Unsubscribe()
    {
        _gameManager.GameTimeTick -= OnGameTimeTick;
        _gameManager.ScoreMulitplierActivated -= OnScoreMultiplierActivated;
        _gameManager.GameEnded -= OnGameEnded;
    }

    #endregion

    #region Public Methods

    public void ToggleHUD(bool isVisible)
    {
        _gameTimer.DOAnchorPosY(isVisible ? TIMER_VISIBLE_Y_POSITION : _timerHiddenYPosition, 1.0f);
    }

    #endregion

    #region Event Handlers

    private void OnGameTimeTick(int timeRemaining)
    {
        _gameTimerText.text = $"{timeRemaining / 60:D2}:{timeRemaining % 60:D2}";
    }

    private void OnScoreMultiplierActivated()
    {
        _gameTimerText.color = Color.red;
        _gameTimer.DOScale(1.05f, 0.55f)
                  .SetLoops(-1, LoopType.Yoyo)
                  .SetEase(Ease.InOutSine)
                  .SetId(TWEEN_ID_TIMER_SCALE_PULSE);
    }

    private void OnGameEnded()
    {
        DOTween.Kill(TWEEN_ID_TIMER_SCALE_PULSE);
        if (_gameTimer.localScale != Vector3.one)
        {
            _gameTimer.DOScale(1.0f, 1.0f);
        }

        ToggleHUD(false);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
