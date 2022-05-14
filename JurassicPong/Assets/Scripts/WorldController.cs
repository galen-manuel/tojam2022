using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public static readonly Vector4 WORLD_BOUNDS = new Vector4(0.5f, 17.78371f, 1f, 9f);

    [Header("Backgrounds")]
    [SerializeField] private RectTransform _backgroundCanvas;
    [SerializeField] private Image _leftWorldBackground;
    [SerializeField] private Image _rightWorldBackground;
    [SerializeField] private Image _worldSeam;
    [SerializeField] private Transform _worldSeamCollider;


    [Header("Scoring")]
    public int MaxScoreDifference = 10;
    public float MaxScreenPercentage = 25f;

    private float _percentagePerPoint;
    private float _playerOneScore;
    private float _playerTwoScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _percentagePerPoint = (1f / MaxScoreDifference * MaxScreenPercentage) / 100f;
        // Player one score.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ScorePlayerOne(1);
        }

        // Player two scoring.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ScorePlayerTwo(1);
        }
    }

    public void ScorePlayerOne(int delta)
    {
        _playerOneScore += delta;
        OnPlayerScored(true, delta);
    }

    public void ScorePlayerTwo(int delta)
    {
        _playerTwoScore += delta;
        OnPlayerScored(false, delta);
    }

    public void OnPlayerScored(bool isPlayerOne, int delta)
    {
        float movementPerc = ((float)delta / MaxScoreDifference * MaxScreenPercentage) / 100f;
        if (isPlayerOne)
        {
            _leftWorldBackground.fillAmount += movementPerc;
            _rightWorldBackground.fillAmount -= movementPerc;
            _worldSeam.rectTransform.anchoredPosition += new Vector2(_backgroundCanvas.sizeDelta.x * movementPerc, 0f);
            _worldSeamCollider.transform.position += new Vector3(WORLD_BOUNDS.y * movementPerc, 0f, 0f);
        }
        else
        {
            _leftWorldBackground.fillAmount -= movementPerc;
            _rightWorldBackground.fillAmount += movementPerc;
            _worldSeam.rectTransform.anchoredPosition -= new Vector2(_backgroundCanvas.sizeDelta.x * movementPerc, 0f);
            _worldSeamCollider.transform.position -= new Vector3(WORLD_BOUNDS.y * movementPerc, 0f, 0f);
        }
    }
}
