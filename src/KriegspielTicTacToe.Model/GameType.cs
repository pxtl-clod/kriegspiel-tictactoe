namespace KriegspielTicTacToe.Model;

public abstract record GameType<TBoard>() : IGameType
where TBoard : Board {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<TBoard> ConstructBoards();
}