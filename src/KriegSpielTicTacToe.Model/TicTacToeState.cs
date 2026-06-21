using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model;

/// <summary>Represents a game state for Kriegspiel Tic Tac Toe.</summary>
public record TicTacToeState {
    /// <summary>Default constructor - uses basic GameType configuration.</summary>
    public TicTacToeState() : this(new GameType(
        new BoardBuilder[2] { 
            new((sbyte)3, (sbyte)3),
            new((sbyte)4, (sbyte)4)
        }, isSynchronousMode: false)) { }

    /// <summary>Constructor accepting player marks and GameType.</summary>
    [JsonConstructor]
    public TicTacToeState(
        char[] players,
        GameType gameType
    ) {
        Players = players.Select(c => new Player(c.ToString())).ToArray();
        PlayManager = (gameType.IsSynchronousMode)
            ? new SynchronizedPlayManager(Players.AsReadOnly())
            : new RoundRobinPlayManager(Players.AsReadOnly());
        Boards = gameType.BoardBuilders.Select(b => new Board(b)).ToList();
        Initialize();
    }

    /// <summary>Constructor for explicit players and GameType configuration.</summary>
    public TicTacToeState(
        Player[] players,
        GameType gameType
    ) {
        Players = players;
        PlayManager = (gameType.IsSynchronousMode)
            ? new SynchronizedPlayManager(Players.AsReadOnly())
            : new RoundRobinPlayManager(Players.AsReadOnly());
        Boards = gameType.BoardBuilders.Select(b => new Board(b)).ToList();
        Initialize();
    }

    /// <summary>Legacy constructor for backwards compatibility.</summary>
    public TicTacToeState(
        Player[] players,
        IEnumerable<BoardBuilder> boardBuilders,
        bool isRandomPlayerOrder,
        bool isSynchronousMode
    ) {
        PlayManager = (isSynchronousMode)
            ? new SynchronizedPlayManager(players.AsReadOnly())
            : new RoundRobinPlayManager(players.AsReadOnly());
        Boards = boardBuilders.Select(b => new Board(b)).ToList();
        Initialize(isRandomizeOrder: isRandomPlayerOrder);
    }

    public void Initialize() {
        PlayActionBuffer?.GameState = this;
    }

    public void Initialize(bool isRandomizeOrder) 
    {
        PlayActionBuffer?.GameState = this;
        if(isRandomizeOrder && (Players as Player[]).Length > 0) {
            Random.Shared.Shuffle((Player[])Players);
        }
    }

    public IReadOnlyList<Player> Players {get;}
    public PlayManager PlayManager {get;}
    public IReadOnlyList<Board> Boards {get;init;}
    public PlayActionBuffer? PlayActionBuffer {get;internal init;} = null!;

    [JsonIgnore]
    public int NumberOfActivePlayers => PlayManager.ActivePlayers.Count();

    [JsonIgnore]
    public Player? Winner {
        get {
            if (!IsGameOver) return null;
            var active = PlayManager.ActivePlayers.ToList();
            return active?.Count == 1 ? active[0] : null;
        }
    }

    [JsonIgnore]
    public bool IsGameOver => BoardDoneCount >= Boards.Count 
        || NumberOfActivePlayers <= 1;

    internal int BoardDoneCount {get;}

    public string ResignedPlayersText 
        => PlayManager.ResignedPlayersSet.Count > 0
            ? $"Resigned players: {string.Join(", ", PlayManager.ResignedPlayersSet.OrderBy(p => p.Mark))}"
            : "";

    [JsonIgnore]
    public string GameStateText {
        get {
            var winner = Winner;
            if (winner != null) return $"Game over. {winner} wins";
            
            if (!IsGameOver || NumberOfActivePlayers < 1) 
                return PlayManager.GameStateText + Environment.NewLine + ResignedPlayersText;
                
            return "Game over. Tie game.";
        }
    }

    public ScoreCard ScoreCard => new();
}

/// <summary>Already played indicator.</summary>
public struct AlreadyPlayed;

/// <summary>Board is done.</summary>
public struct BoardIsDone;

/// <summary>Action queued successfully.</summary>
public struct ActionQueuedSuccessfully;
