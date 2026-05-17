namespace KriegspielTicTacToe.Model;

/// <summary>
/// Action to be executed when playing a space.
/// </summary>
public record TicTacToePlayAction(
    int BoardIndex,
    int Col,
    int Row,
    char Player
);