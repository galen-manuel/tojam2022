using UnityEngine;
using TMPro;
using System;

public class HUDManager : MonoBehaviour
{
    #region Private Variables

    [Header("Misc")]
    [SerializeField] private GameManager _gameManager;

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI _gameTimer;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_gameManager);
        GameHelper.IsNull(_gameTimer);

        Unsubscribe();
        Subscribe();
    }
    #region Private Methods

    private void Subscribe()
    {
        _gameManager.GameTimeTick += OnGameTimeTick;
        _gameManager.ScoreMulitplierActivated += OnScoreMultiplierActivated;
    }

    private void Unsubscribe()
    {
        _gameManager.GameTimeTick -= OnGameTimeTick;
        _gameManager.ScoreMulitplierActivated -= OnScoreMultiplierActivated;
    }

    #endregion

    #region Event Handlers

    private void OnGameTimeTick(int timeRemaining)
    {
        _gameTimer.text = $"{timeRemaining / 60:D2}:{timeRemaining % 60:D2}";
    }

    private void OnScoreMultiplierActivated()
    {
        _gameTimer.color = Color.red;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
