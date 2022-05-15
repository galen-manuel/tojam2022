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

    /// <summary>
    /// int - Time Remaining
    /// </summary>
    public Action<int> GameTimeTick;

    /// <summary>
    /// int - Score delta
    /// <para><see cref="Portal.Side"/> - Which portal was scored on.</para>
    /// <para><see cref="Thing"/> - What type of object was scored (<see cref="Thing"/> is a good thing, 
    /// <see cref="BadThing"/> is not.)</para>
    /// </summary>
    public Action<Portal.Side, int, Thing> Scored;

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

        if (adjustTimeScale)
        {
            Time.timeScale = timeScale;
        }

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

        Scored?.Invoke(side, delta, thing);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
