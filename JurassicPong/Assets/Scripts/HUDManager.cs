using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class HUDManager : MonoBehaviour
{
    #region Constants

    private const float TIMER_VISIBLE_Y_POSITION = 2f;
    private const string TWEEN_ID_TIMER_SCALE_PULSE = "TweenIDTimerScalePulse";

    #endregion

    #region Private Variables

    [Header("HUD Elements")]
    [SerializeField] private RectTransform _gameTimer;
    [SerializeField] private TextMeshProUGUI _gameTimerText;
    public CountdownSequence CountdownSequence;

    private float _timerHiddenYPosition;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_gameTimer);
        GameHelper.IsNull(_gameTimerText);

        _timerHiddenYPosition = _gameTimer.rect.height;

        Subscribe();
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener(Events.COUNTDOWN_STARTED, OnStartCountdown);
        Messenger.AddListener<int>(Events.START_GAME, OnStartGame);
        Messenger.AddListener<int>(Events.GAME_TIME_TICK, OnGameTimeTick);
        Messenger.AddListener(Events.SCORE_MULTIPLIER_ACTIVATED, OnScoreMultiplierActivated);
        Messenger.AddListener(Events.GAME_OVER, OnGameEnded);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener(Events.COUNTDOWN_STARTED, OnStartCountdown);
        Messenger.RemoveListener<int>(Events.START_GAME, OnStartGame);
        Messenger.RemoveListener<int>(Events.GAME_TIME_TICK, OnGameTimeTick);
        Messenger.RemoveListener(Events.SCORE_MULTIPLIER_ACTIVATED, OnScoreMultiplierActivated);
        Messenger.RemoveListener(Events.GAME_OVER, OnGameEnded);
    }

    private void SetTimer(int timeRemaining)
    {
        _gameTimerText.text = $"{timeRemaining / 60:D2}:{timeRemaining % 60:D2}";
    }

    #endregion

    #region Public Methods

    public void ToggleHUD(bool isVisible)
    {
        _gameTimer.DOAnchorPosY(isVisible ? TIMER_VISIBLE_Y_POSITION : _timerHiddenYPosition, 1.0f);
    }

    #endregion

    #region Event Handlers

    private void OnStartCountdown()
    {
        CountdownSequence.StartCountdown(OnCountdownSequenceComplete);
    }

    private void OnStartGame(int startingGameTime)
    {
        SetTimer(startingGameTime);
        ToggleHUD(true);
    }

    private void OnCountdownSequenceComplete()
    {
        Messenger.Broadcast(Events.COUNTDOWN_ENDED);
    }

    private void OnGameTimeTick(int timeRemaining)
    {
        SetTimer(timeRemaining);
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
        DOTween.Kill(TWEEN_ID_TIMER_SCALE_PULSE);
        Unsubscribe();
    }

    #endregion
}
