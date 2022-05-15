using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Variables

    [SerializeField] private Portal[] _portals;

    private int _playerOneScore;
    private int _playerTwoScore;
    private int _timeRemaining;
    private int _activePointsMultiplier;
    private bool _isPointsMultiplierActive;

    #endregion

    #region Public Variables

    [Tooltip("Value in seconds.")]
    public int TotalMatchTime = 3 * 60;

    [Tooltip("Time in seconds at which points will increase in value.")]
    public int PointsMultiplierTime = 30;

    [Tooltip("Multipler that will take effect when the clock runs down to the multiplier time.")]
    public int PointsMultiplier = 2;

    /// <summary>
    /// int - Time Remaining
    /// </summary>
    public Action<int> GameTimeTick;

    /// <summary>
    /// int - Score delta
    /// <para><see cref="Portal.Side"/> - Which portal was scored on.</para>
    /// </summary>
    public Action<Portal.Side, int> Scored;

    public Action GameEnded;
    public Action ScoreMulitplierActivated;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_portals);

        DontDestroyOnLoad(this);

        Unsubscribe();
        Subscribe();

        _timeRemaining = TotalMatchTime;
        _activePointsMultiplier = 1;

        StartCoroutine(GameTimeTicker());
    }

    #region Private Methods

    private void Subscribe()
    {
        foreach (Portal portal in _portals)
        {
            portal.Scored += OnScored;
        }
    }

    private void Unsubscribe()
    {
        foreach (Portal portal in _portals)
        {
            portal.Scored -= OnScored;
        }
    }

    private void Score(Portal.Side side, string tag, ref int playerScore)
    {
        // Apply the multiplier first.
        int delta = 1 * _activePointsMultiplier;

        // Make the delta negative for bad things.
        if (tag == Constants.TAG_BAD_THING)
        {
            delta *= -1;
        }

        playerScore += delta;
        Scored?.Invoke(side, delta);
    }

    #endregion

    #region Coroutines

    private IEnumerator GameTimeTicker()
    {
        while (_timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            _timeRemaining -= 1;
            GameTimeTick?.Invoke(_timeRemaining);

            if (_timeRemaining <= PointsMultiplierTime && !_isPointsMultiplierActive)
            {
                _isPointsMultiplierActive = true;
                _activePointsMultiplier = PointsMultiplier;
                ScoreMulitplierActivated?.Invoke();
            }
        }

        if (_timeRemaining == 0)
        {
            Debug.Log("GAME OVER!");
            GameEnded?.Invoke();
        }
    }

    #endregion

    #region Event Handlers

    private void OnScored(Portal.Side side, string tag)
    {
        Score(side, tag, ref side == Portal.Side.Left ? ref _playerOneScore : ref _playerTwoScore);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
