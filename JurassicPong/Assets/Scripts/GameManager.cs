using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static RoundResultsModel;

public class GameManager : MonoBehaviour
{
    #region Private Variables

    [Header("Scoring Properties Data")]
    [SerializeField] private ScoringPropertiesData _scoringPropertiesData;
    [SerializeField] private AudioSource _gameAudio;
    private int _playerOneScore;
    private int _playerTwoScore;
    private int _timeRemaining;
    private int _activePointsMultiplier;
    private bool _isPointsMultiplierActive;
    private Coroutine _gameTimer;

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

    [Space]
    public PlayMusic PlayMusic;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_scoringPropertiesData);
        GameHelper.IsNull(_gameAudio);

        Subscribe();

        _timeRemaining = TotalMatchTime;
        _activePointsMultiplier = 1;

        if (adjustTimeScale)
        {
            Time.timeScale = timeScale;
        }
    }

    private void Start()
    {
        Messenger.Broadcast(Events.COUNTDOWN_STARTED);
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener(Events.COUNTDOWN_ENDED, OnCountdownEnded);
        Messenger.AddListener<Portal.Side, Thing>(Events.PORTAL_SCORED, OnScored);
        Messenger.AddListener(Events.GAME_OVER_ANIMATION_COMPLETE, OnGameOverAnimationComplete);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener(Events.COUNTDOWN_ENDED, OnCountdownEnded);
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
                _gameAudio.pitch = 1.1f;
                Messenger.Broadcast(Events.SCORE_MULTIPLIER_ACTIVATED, MessengerMode.REQUIRE_LISTENER);
            }
        }

        CheckGameOver();
    }

    private float CalculateProgress(int score1, int score2)
    {
        if (score1 > score2)
        {
            return 1.0f;
        }

        return (float)score1 / score2;
    }

    private ScoreEndState DetermineMatchWinner()
    {
        if (_playerOneScore > _playerTwoScore)
        {
            return ScoreEndState.P1;
        }

        if (_playerTwoScore > _playerOneScore)
        {
            return ScoreEndState.P2;
        }

        return ScoreEndState.Tie;
    }

    private void CheckGameOver()
    {
        int scoreDifference = Mathf.Abs(_playerOneScore - _playerTwoScore);
        if (_timeRemaining == 0 || scoreDifference >= _scoringPropertiesData.MaxScoreDifference)
        {
            Debug.Log("Game Over Broadcast");
            Messenger.Broadcast(Events.GAME_OVER);
            StopCoroutine(_gameTimer);
        }
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
        CheckGameOver();
    }

    private void OnGameOverAnimationComplete()
    {
        var roundResultsModel = new RoundResultsModel()
        {
            PlayerOneScore = _playerOneScore,
            PlayerTwoScore = _playerTwoScore,
            MatchWinner = DetermineMatchWinner(),
            PlayerOneOverallProgress = CalculateProgress(_playerOneScore, _playerTwoScore),
            PlayerTwoOverallProgress = CalculateProgress(_playerTwoScore, _playerOneScore)
        };
        Messenger.Broadcast(Events.GAME_OVER_RESULTS, roundResultsModel, MessengerMode.REQUIRE_LISTENER);
    }

    private void OnCountdownEnded()
    {
        PlayMusic.PlayLevelMusic();
        Messenger.Broadcast(Events.START_GAME, TotalMatchTime);
        _gameTimer = StartCoroutine(GameTimeTicker());
    }

    private void OnDestroy()
    {
        StopCoroutine(_gameTimer);
        Unsubscribe();
    }

    #endregion
}
