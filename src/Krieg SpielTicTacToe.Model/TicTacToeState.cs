using OneOf;
using System.Collections.Generic;

namespace KriegspielTicTacToe.Model;

/// <summary>Represents a game state for Kriegspiel Tic Tac Toe.</summary>
public record TicTacToeState {
    /// <summary>Default constructor - uses basic GameType configuration.</summary>
    public TicTacToeState() : this(new[] { "X", "O" }, new GameType(
        new BoardBuilder[2] { 
            new((sbyte)3, (sbyte)3),
            new((sbyte)4, (sbyte)4)
        }, false)) { }

    /// <summary>Constructor accepting player marks and GameType.</summary>
    [JsonConstructor]
    public TicTacToeState(
        string[] players,
        GameType gameType
    ) {
        char[] marks = players.Select(s => s[0]).ToArray();
        PlayManager = (gameType.IsSynchronousMode)
            ? new SynchronizedPlayManager(marks.ToArray().Select(char.ToString).Select(Player.Parse).ToArray())
            : new RoundRobinPlayManager(marks.ToArray().Select(char.ToString).Select(Player.Parse).ToArray());
        Boards = gameType.BoardBuilders.Select(b => new Board(b)).ToList();
    }

    /// <summary>Constructor for explicit players and GameType configuration.</summary>
    public TicTacToeState(
        Player[] players,
        GameType gameType
    ) {
        PlayManager = (gameType.IsSynchronousMode)
            ? new SynchronizedPlayManager(players.AsReadOnly())
            : new RoundRobinPlayManager(players.AsReadOnly());
        Boards = gameType.BoardBuilders.Select(b => new Board(b)).ToList();
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
        Initialize();
    }

    private void Initialize() {
        PlayActionBuffer?.GameState = this;
    }

    public IReadOnlyList<Player> Players {get;}
    public PlayManager PlayManager {get;}
    public IReadOnlyList<Board> Boards {get;init;}
    [JsonIgnore]
    public int NumberOfActivePlayers => PlayManager is null ? 0 : PlayManager.ActivePlayers.Count();

    [JsonIgnore]
    public Player? Winner {
        get {
            if (!IsGameOver) return null;
            var active = PlayManager?.ActivePlayersToList();
            return active?.Count == 1 ? active[0] : null;
        }
    }

    [JsonIgnore]
    public bool IsGameOver => false;

    internal void SetBoardDone(int count) { BoardDoneCount = count; }
    internal int BoardDoneCount {get;}

    private readonly List<Player> _players = [];
    
    public string ResignedPlayersText 
        => PlayManager?.ResignedPlayersSet.Count > 0
            ? $"Resigned players: {string.Join(", ", PlayManager.ResignedPlayersSet.OrderBy(p => p.Mark))}"
            : "";

    [JsonIgnore]
    public ScoreCard ScoreCard => new _empty();
}

private static readonly IReadOnlyList<PlayerScore> _empty = [];

public struct AlreadyPlayed;
public struct BoardIsDone {get;set;}
public struct ActionQueuedSuccessfully {}