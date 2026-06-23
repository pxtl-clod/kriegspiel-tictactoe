namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToePlayAction : PlayAction<TicTacToePlayAction, TicTacToeState> {
    [Obsolete("Default constructor is only used for deserialization.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public TicTacToePlayAction() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public TicTacToePlayAction(
        int boardIndex,
        int col,
        int row,
        Player player
    ) {
        BoardIndex = boardIndex;
        Col = col;
        Row = row;
        Player = player;
    }
    public int BoardIndex {get;set;}
    public int Col {get;set;}
    public int Row {get;set;}
    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    Player Player {get;set;}

	public override void DoActionCollision(TicTacToeState gameState) {
        if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);
		space.Mark = "█";
        foreach(var player in gameState.PlayManager.Players) {
            space.MakeKnownToPlayer(player);    
        }
	}

    protected Board GetBoard(TicTacToeState gameState)
        => gameState.GetBoardByIndex(BoardIndex);

    protected Space GetSpace(TicTacToeState gameState)
        => GetBoard(gameState).Spaces[Col, Row];

	public override bool IsActionCollision(TicTacToePlayAction otherAction)
    => BoardIndex == otherAction.BoardIndex
        && Col == otherAction.Col
        && Row == otherAction.Row
        && Player != otherAction.Player;

	public override void DoAction(TicTacToeState gameState)
	{
		if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);

        if (space.Mark == null) {
            space.Mark = Player.Mark;
        }
            
        space.MakeKnownToPlayer(Player);
	}
}
