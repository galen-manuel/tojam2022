using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverDialog : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _statsWindow;

    [Header("Player 1 Components")]
    [SerializeField] private GameObject _leftVictoryText;
    [SerializeField] private TextMeshProUGUI _leftScoreText;
    [SerializeField] private Image _leftProgressBar;

    [Header("Player 2 Components")]
    [SerializeField] private GameObject _rightVictoryText;
    [SerializeField] private TextMeshProUGUI _rightScoreText;
    [SerializeField] private Image _rightProgressBar;

    private void Awake()
    {
        GameHelper.IsNull(_background);
        GameHelper.IsNull(_statsWindow);
        GameHelper.IsNull(_leftVictoryText);
        GameHelper.IsNull(_leftScoreText);
        GameHelper.IsNull(_leftProgressBar);
        GameHelper.IsNull(_rightVictoryText);
        GameHelper.IsNull(_rightScoreText);
        GameHelper.IsNull(_rightProgressBar);

        Subscribe();
    }

    private void Subscribe()
    {
        Messenger.AddListener<RoundResultsModel>(Constants.EVENTS_GAME_OVER_RESULTS, OnResultsTallied);
    }

    private void OnResultsTallied(RoundResultsModel resultsModel)
    {
        _background.gameObject.SetActive(true);
        _statsWindow.gameObject.SetActive(true);

        _leftVictoryText.SetActive(resultsModel.IsPlayerOneWinner);
        _rightVictoryText.SetActive(!resultsModel.IsPlayerOneWinner);

        _leftScoreText.text = $"{resultsModel.PlayerOneScore:D3}";
        _rightScoreText.text = $"{resultsModel.PlayerTwoScore:D3}";

        _leftProgressBar.fillAmount = resultsModel.PlayerOneOverallProgress;
        _rightProgressBar.fillAmount = resultsModel.PlayerTwoOverallProgress;
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<RoundResultsModel>(Constants.EVENTS_GAME_OVER_RESULTS, OnResultsTallied);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}