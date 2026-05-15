using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model;

public class PlayManager
{
    #region members
    public IReadOnlyList<char> Players {get;init;} = new List<char>();

    private int _currentTurnPlayerIndex = 0;

    /// <summary>
    /// index within list of *active* players - does not include resigned players.
    /// </summary>
    public int CurrentTurnPlayerIndex {get; private set;}

    private int _roundIndex = 0;
    public int RoundIndex {
        get { return _roundIndex; }
        set { _roundIndex = value; }
    }

    public bool IsNewRound { get; private set;}

    public bool IsSynchronousMode {get; init;} = false;
    
    public HashSet<char> ResignedPlayersSet {get;init;} = new HashSet<char>();
    #endregion

    #region methods
    /// <summary>
    /// Advance to the next player's turn.  Must notify if current player is
    /// resigning because that effects how the turn counter is incremented.
    /// </summary>
    /// <remarks>
    /// The logic here is tricky. If the current player resigns, then their slot
    /// is removed from the index and we skip incrementing.  But that means
    /// there are 2 "index 0" turns, so we can't use "index 0" as new round in
    /// that case.
    /// </remarks>
    public void NextTurn(bool isCurrentPlayerResigning) {
        if (!isCurrentPlayerResigning) {
            CurrentTurnPlayerIndex += 1;
        }
        if (IsCurrentTurnPlayerIndexOutOfRange) {
            RoundIndex += 1;
            CurrentTurnPlayerIndex = 0;
            IsNewRound = true;
        } else {
            IsNewRound = false;
        }
    }
    
    /// <summary>
    /// Test if the given player has resigned.
    /// </summary>
    public bool IsResignedPlayer(char player)
        => ResignedPlayersSet.Contains(player);

    /// <summary>
    /// Mark the given player as resigned. If it is the current player's turn,
    /// do *not* call NextTurn.
    /// </summary>
    public void ResignPlayer(char player) {
        ResignedPlayersSet.Add(player);
    }

    /// <summary>
    /// True if the given player is able to take a turn.
    /// </summary>
    public bool CanTakeTurn(char? player)
        => CurrentTurnPlayer.Match(
            result => result.Value == player,
            currentTurnPlayerIndexOutOfRange => false
        );

    internal void CheckIsValidToSave()
    {
        if (IsCurrentTurnPlayerIndexOutOfRange) {
            throw new InvalidOperationException(
                "Cannot save when current turn index player is out of range."
            );
        }
    }
    #endregion

    #region helper properties
    [JsonIgnore()]
    public int NumberOfActivePlayers
        => ActivePlayers.Count();
    
    /// <summary>
    /// Get all of the current active players.  Order is consistent.
    /// </summary>
    [JsonIgnore()]
    public IEnumerable<char> ActivePlayers
        => Players.Except(ResignedPlayersSet);

    /// <summary>
    /// Get the mark-char of the current-turn player.
    /// </summary>
    [JsonIgnore()]
    public OneOf<Result<char>, CurrentTurnPlayerIndexOutOfRange> CurrentTurnPlayer 
        => IsCurrentTurnPlayerIndexOutOfRange
        ? new CurrentTurnPlayerIndexOutOfRange()
        : new Result<char>(ActivePlayers.ElementAt(CurrentTurnPlayerIndex));

    /// <summary>
    /// This can happen momentarily if the final player resigns.
    /// </summary>
    public bool IsCurrentTurnPlayerIndexOutOfRange
        => CurrentTurnPlayerIndex >= ActivePlayers.Count();

    [JsonIgnore()]
    public string GameStateText
        =>  $"Player '{CurrentTurnPlayer}' turn.";


    #endregion
}

/// <summary>
/// Empty struct for OneOf indicating that the current player has retired.
/// </summary>
public record CurrentTurnPlayerIndexOutOfRange;