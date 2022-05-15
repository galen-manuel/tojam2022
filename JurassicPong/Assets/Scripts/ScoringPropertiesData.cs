using UnityEngine;

[CreateAssetMenu(fileName = "ScoringPropertiesData", menuName = "Game Data/Scoring Properties")]
public class ScoringPropertiesData : ScriptableObject
{
    [Header("Scoring Properties")]
    [Tooltip("The maximum score difference before the game automatically ends.")]
    public int MaxScoreDifference = 10;

    [Tooltip("The maximum screen real estate represented as a percentage of 50%. Example: A value of 25% will mean" +
        "that any player's world can leak 25% into the other player's world covering 75% of the screen. Base " +
        "value is 50%.")]
    public float MaxScreenPercentage = 25f;
}
