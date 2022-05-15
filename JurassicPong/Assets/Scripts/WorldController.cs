using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public static readonly Vector4 WORLD_BOUNDS = new Vector4(0.5f, 17.78371f, 1f, 9f);

    #region Private Variables

    [Header("Backgrounds")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RectTransform _backgroundCanvas;
    [SerializeField] private Image _leftWorldBackground;
    [SerializeField] private Image _rightWorldBackground;
    [SerializeField] private Image _worldSeam;
    [SerializeField] private Transform _worldSeamCollider;
    private float _percentagePerPoint;

    #endregion

    #region Public Variables

    [Header("Scoring")]
    public int MaxScoreDifference = 10;
    public float MaxScreenPercentage = 25f;

    #endregion

    private void Awake()
    {
        GameHelper.IsNull(_gameManager);
        GameHelper.IsNull(_backgroundCanvas);
        GameHelper.IsNull(_leftWorldBackground);
        GameHelper.IsNull(_rightWorldBackground);
        GameHelper.IsNull(_worldSeam);
        GameHelper.IsNull(_worldSeamCollider);

        Unsubscribe();
        Subscribe();
    }

    private void Update()
    {
        _percentagePerPoint = (1f / MaxScoreDifference * MaxScreenPercentage) / 100f;
    }

    #region Private Methods

    private void Subscribe()
    {
        _gameManager.Scored += OnPlayerScored;
    }

    private void Unsubscribe()
    {
        _gameManager.Scored -= OnPlayerScored;
    }

    #endregion

    #region Event Handlers

    private void OnPlayerScored(Portal.Side side, int delta)
    {
        float movementPerc = ((float)delta / MaxScoreDifference * MaxScreenPercentage) / 100f;

        switch (side)
        {
            case Portal.Side.Left:
                _leftWorldBackground.fillAmount += movementPerc;
                _rightWorldBackground.fillAmount -= movementPerc;
                _worldSeam.rectTransform.anchoredPosition += new Vector2(_backgroundCanvas.sizeDelta.x * movementPerc, 0f);
                _worldSeamCollider.transform.position += new Vector3(WORLD_BOUNDS.y * movementPerc, 0f, 0f);
                break;
            case Portal.Side.Right:
                _leftWorldBackground.fillAmount -= movementPerc;
                _rightWorldBackground.fillAmount += movementPerc;
                _worldSeam.rectTransform.anchoredPosition -= new Vector2(_backgroundCanvas.sizeDelta.x * movementPerc, 0f);
                _worldSeamCollider.transform.position -= new Vector3(WORLD_BOUNDS.y * movementPerc, 0f, 0f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side));
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
