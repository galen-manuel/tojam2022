using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameOverDialog : MonoBehaviour
{
    private const string TWEEN_ID_TALLYING_SEQ = "TallyingSequence";
    private const float TWEEN_DURATION_STATS_TALLYING = 3.5f;

    [Header("General")]
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _statsWindow;
    [SerializeField] private RectTransform _playAgainBtn;
    [SerializeField] private RectTransform _mainMenuBtn;
    [SerializeField] private RectTransform _tieText;

    [Header("Player 1 Components")]
    [SerializeField] private RectTransform _leftVictoryText;
    [SerializeField] private TextMeshProUGUI _leftScoreText;
    [SerializeField] private Image _leftProgressBar;

    [Header("Player 2 Components")]
    [SerializeField] private RectTransform _rightVictoryText;
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
        Messenger.AddListener<RoundResultsModel>(Events.GAME_OVER_RESULTS, OnResultsTallied);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<RoundResultsModel>(Events.GAME_OVER_RESULTS, OnResultsTallied);
    }

    private void OnResultsTallied(RoundResultsModel resultsModel)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        int initialP1Value = 0;
        int initialP2Value = 0;
        float tweenDuration = 0f;

        Sequence seq = DOTween.Sequence();
        seq.SetId(TWEEN_ID_TALLYING_SEQ);

        seq.Insert(0f, _background.DOFade(200.0f/255.0f, 0.5f).SetEase(Ease.InSine));
        tweenDuration += 0.5f;

        seq.Insert(tweenDuration * 0.5f, _statsWindow.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack));
        tweenDuration += 0.25f;

        seq.Insert(tweenDuration, DOTween.To(() => initialP1Value, x => _leftScoreText.text = $"{x:D3}", 
            resultsModel.PlayerOneScore, TWEEN_DURATION_STATS_TALLYING).SetEase(Ease.InCirc));
        seq.Insert(tweenDuration, DOTween.To(() => initialP2Value, x => _rightScoreText.text = $"{x:D3}",
            resultsModel.PlayerTwoScore, TWEEN_DURATION_STATS_TALLYING).SetEase(Ease.InCirc));
        seq.Insert(tweenDuration, _leftProgressBar.DOFillAmount(resultsModel.PlayerOneOverallProgress, TWEEN_DURATION_STATS_TALLYING)
                                       .SetEase(Ease.InCirc));
        seq.Insert(tweenDuration, _rightProgressBar.DOFillAmount(resultsModel.PlayerTwoOverallProgress, TWEEN_DURATION_STATS_TALLYING)
                                        .SetEase(Ease.InCirc));
        tweenDuration += TWEEN_DURATION_STATS_TALLYING;

        switch (resultsModel.MatchWinner)
        {
            case RoundResultsModel.ScoreEndState.Tie:
                seq.Insert(tweenDuration, _tieText.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack));
                break;
            case RoundResultsModel.ScoreEndState.P1:
                seq.Insert(tweenDuration, _leftVictoryText.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack));
                break;
            case RoundResultsModel.ScoreEndState.P2:
                seq.Insert(tweenDuration, _rightVictoryText.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack));
                break;
            default:
                break;
        }

        tweenDuration += 0.5f;
        seq.Insert(tweenDuration, _playAgainBtn.DOScale(1.0f, 0.5f).SetEase(Ease.OutSine));
        seq.Insert(tweenDuration, _mainMenuBtn.DOScale(1.0f, 0.5f).SetEase(Ease.OutSine));
    }

    private void OnDestroy()
    {
        DOTween.Kill(TWEEN_ID_TALLYING_SEQ);
        Unsubscribe();
    }

    public void OnPlayAgainPressed()
    {
        SceneManager.LoadScene(1);
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene(0);
    }
}
