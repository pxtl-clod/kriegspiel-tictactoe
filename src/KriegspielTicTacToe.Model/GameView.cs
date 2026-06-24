namespace KriegspielTicTacToe.Model;

public record GameView {
    #region Constructors
    public GameView (Player? player, IGameState gameState) {
        Player = player;
        GameState = gameState;
    }
    #endregion

    #region Data Properties
    public Player? Player {get; init;}
    protected IGameState GameState { get; init; }
    #endregion

    #region Calculated Members
    public IReadOnlyList<Board> Boards => GameState.Boards;
    public bool IsGameOver => GameState.IsGameOver;
    public bool CanTakeTurn => GameState.PlayManager.CanTakeTurn(Player);
    #endregion
}