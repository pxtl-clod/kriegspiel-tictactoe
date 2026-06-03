namespace KriegspielTicTacToe.Model;

using Sundew.Base.Collections.Immutable;
using OneOf.Types;

/// <summary>
/// Immutable value-y collection of scores
/// </summary>
public record struct ScoreCard {
    #region constructors
    public ScoreCard() {
        Scores = _emptyPlayerScoreCollection;
    }
    public ScoreCard(Player player, int score) : this(new PlayerScore(player, score)) {}
    public ScoreCard(PlayerScore playerScore) : this([playerScore]) {}
    public ScoreCard(IEnumerable<PlayerScore> scores) {
        Scores = scores
            .GroupBy(s => s.Player)
            .Select(g => new PlayerScore(g.Key, g.Sum(kvp => kvp.Score)))
            .ToValueArray();
    }

    #endregion

    #region static Empty
    private static ScoreCard _empty = new ScoreCard();
    private static ValueArray<PlayerScore> _emptyPlayerScoreCollection = new ValueArray<PlayerScore>();
    public static ScoreCard Empty
        => _empty;

    #endregion

    #region state members
    private ValueArray<PlayerScore> Scores {get;set;}
    #endregion

    #region calculated members
    public bool IsEmpty
        => Scores.Count == 0;

    public PlayerScore? HighestScore
        => Scores.Count == 0 
            ? null
            : Scores.MaxByStrict(s => s.Score);
            
    #endregion

    #region overloads
    public static ScoreCard operator +(ScoreCard a, ScoreCard b)
        => a.IsEmpty ? b // optimization, if a or b are empty just use the other one directly.
        : b.IsEmpty ? a
        : new ScoreCard(a.Scores.Concat(b.Scores));

    public static ScoreCard operator +(ScoreCard a, PlayerScore b)
        => new ScoreCard(a.Scores.Append(b));

    public static ScoreCard operator +(PlayerScore a, ScoreCard b)
        => new ScoreCard(a) + b;
    #endregion

    public static ScoreCard SumScoreCards(IEnumerable<ScoreCard> scoreCards)
        => new ScoreCard(scoreCards.SelectMany(s => s.Scores));
}

public static class ScoreCardExtensions {
    public static ScoreCard SumScoreCards(this IEnumerable<ScoreCard> scoreCards)
        => ScoreCard.SumScoreCards(scoreCards);
}
