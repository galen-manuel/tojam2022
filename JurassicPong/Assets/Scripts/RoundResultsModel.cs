public class RoundResultsModel
{
    public enum ScoreEndState
    {
        Tie,
        P1,
        P2
    }

    public int PlayerOneScore;
    public int PlayerTwoScore;
    public ScoreEndState MatchWinner;
    public float PlayerOneOverallProgress;
    public float PlayerTwoOverallProgress;
}
