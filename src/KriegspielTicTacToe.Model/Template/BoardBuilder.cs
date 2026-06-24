namespace KriegspielTicTacToe.Model.Template;

/// <summary>
/// Parameters to create a board, including the scoring settings for the board.  Currently only used by the <see cref="TicTacToeTemplate"/>.
/// </summary>
public record BoardBuilder(sbyte Width, sbyte Height, GameRuleset Ruleset = null!) {
    public GameRuleset Ruleset = Ruleset ?? GameRuleset.Empty;
};
