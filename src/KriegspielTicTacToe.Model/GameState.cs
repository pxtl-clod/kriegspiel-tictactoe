using System.Runtime.Serialization;

namespace KriegspielTicTacToe.Model;

public abstract record GameState<TState, TGame, TBoard, TAction> : IGameState
where TState : GameState<TState, TGame, TBoard, TAction>
where TGame : GameType<TBoard>
where TBoard : Board 
where TAction : PlayAction<TAction, TState> {
    public GameState() { 
        // will probably get removed when members are initialized.
        PlayManager = new RoundRobinPlayManager([]);
        Boards = [];
    }
    public GameState(
        Player[] players,
        TGame gameType,
        bool isRandomPlayerOrder
    ) {
        if(isRandomPlayerOrder) {
            Random.Shared.Shuffle(players);
        }

        PlayManager = gameType.PlayManagerFactory.Create(players);
        Boards = gameType.ConstructBoards();
        Initialize();
    }

    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    public virtual PlayManager PlayManager {get;init;}
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
    public virtual IReadOnlyList<TBoard> Boards {get;init;}
    [JsonIgnore()]
    IReadOnlyList<Board> IGameState.Boards => Boards;
    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public PlayActionBuffer<TAction, TState> PlayActionBuffer {get; init;} = new PlayActionBuffer<TAction, TState>();
    public void Initialize() {
        PlayActionBuffer.GameState = (TState)this;
        PlayManager.PlayActionBuffer = PlayActionBuffer;
    }
    [OnDeserialized]
    public void Initialize(StreamingContext context) {
        Initialize();
    }
}
