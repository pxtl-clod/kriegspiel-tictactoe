namespace KriegspielTicTacToe.Model;

public abstract record GameTemplate()
: IGameTemplate {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<Board> ConstructBoards();
}