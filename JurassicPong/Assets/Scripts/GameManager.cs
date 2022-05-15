using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Variables

    private int _playerOneScore;
    private int _playerTwoScore;
    private int _timeRemaining;
    private int _activePointsMultiplier;
    private bool _isPointsMultiplierActive;

    #endregion

    #region Public Variables

    [Header("DEBUG")]
    public bool adjustTimeScale;
    [Range(0f, 2f)]
    public float timeScale;
    [Space]

    [Tooltip("Value in seconds.")]
    public int TotalMatchTime = 3 * 60;

    [Tooltip("Time in seconds at which points will increase in value.")]
    public int PointsMultiplierTime = 30;

    [Tooltip("Multipler that will take effect when the clock runs down to the multiplier time.")]
    public int PointsMultiplier = 2;

    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this);

        Subscribe();

        _timeRemaining = TotalMatchTime;
        _activePointsMultiplier = 1;

        if (adjustTimeScale)
        {
            Time.timeScale = timeScale;
        }

        StartCoroutine(GameTimeTicker());
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener<Portal.Side, Thing>(Events.PORTAL_SCORED, OnScored);
        Messenger.AddListener(Events.GAME_OVER_ANIMATION_COMPLETE, OnGameOverAnimationComplete);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<Portal.Side, Thing>(Events.PORTAL_SCORED, OnScored);
        Messenger.RemoveListener(Events.GAME_OVER_ANIMATION_COMPLETE, OnGameOverAnimationComplete);
    }

    #endregion

    #region Coroutines

    private IEnumerator GameTimeTicker()
    {
        while (_timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            _timeRemaining -= 1;
            Messenger.Broadcast(Events.GAME_TIME_TICK, _timeRemaining, MessengerMode.REQUIRE_LISTENER);

            if (_timeRemaining <= PointsMultiplierTime && !_isPointsMultiplierActive)
            {
                _isPointsMultiplierActive = true;
                _activePointsMultiplier = PointsMultiplier;
                Messenger.Broadcast(Events.SCORE_MULTIPLIER_ACTIVATED, MessengerMode.REQUIRE_LISTENER);
            }
        }

        if (_timeRemaining == 0)
        {
            Messenger.Broadcast(Events.GAME_OVER);
        }
    }

    private float CalculateProgress(int score1, int score2)
    {
        if (score1 > score2)
        {
            return 1.0f;
        }

        return (float)score1 / score2;
    }

    #endregion

    #region Event Handlers

    private void OnScored(Portal.Side side, Thing thing)
    {
        // Apply the multiplier first.
        int delta = 1 * _activePointsMultiplier;

        switch (side)
        {
            case Portal.Side.Left:
                if (thing is BadThing)
                {
                    _playerTwoScore += delta;
                }
                else
                {
                    _playerOneScore += delta;
                }
                break;
            case Portal.Side.Right:
                if (thing is BadThing)
                {
                    _playerOneScore += delta;
                }
                else
                {
                    _playerTwoScore += delta;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side));
        }

        Messenger.Broadcast(Events.UPDATE_SEAM_POSITION, side, delta, thing, MessengerMode.REQUIRE_LISTENER);
    }

    private void OnGameOverAnimationComplete()
    {
        var roundResultsModel = new RoundResultsModel()
        {
            PlayerOneScore = _playerOneScore,
            PlayerTwoScore = _playerTwoScore,
            IsPlayerOneWinner = _playerOneScore > _playerTwoScore,
            PlayerOneOverallProgress = CalculateProgress(_playerOneScore, _playerTwoScore),
            PlayerTwoOverallProgress = CalculateProgress(_playerTwoScore, _playerOneScore)
        };
        Messenger.Broadcast(Events.GAME_OVER_RESULTS, roundResultsModel, MessengerMode.REQUIRE_LISTENER);
    }

    private void OnDestroy()
    {
        StopCoroutine(GameTimeTicker());
        Unsubscribe();
    }

    #endregion
}
