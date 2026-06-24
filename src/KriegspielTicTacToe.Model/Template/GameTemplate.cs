namespace KriegspielTicTacToe.Model.Template;

/// <summary>
/// A game template includes all the metadata needed to describe the type of a
/// game, but not the actual list of players or active state.
/// </summary>
public abstract record GameTemplate()
: IGameTemplate {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<Board> ConstructBoards();
}