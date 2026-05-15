namespace KriegspielTicTacToe.Model;

using OneOf;
using OneOf.Types;

/// <summary>
/// Object that models the full state of the game.  Is serialized into a json file so all players can share reading it.
/// </summary>
public record TicTacToeState {
    #region constructors
    /// <summary>
    /// This constructor is only used by the serializer, never use it.
    /// </summary>
    public TicTacToeState() { 
        Boards = [];
        PlayManager = new PlayManager();
        ActionBuffer = [];
    }

    /// <summary>
    /// Construct a new gamestate object.  Note that there is no protection at this level against impossible values.
    /// </summary>
    public TicTacToeState(
        char[] players,
        IEnumerable<BoardBuilder> boardBuilders,
        bool isRandomPlayerOrder,
        bool isSynchronousMode
    ) {
        if(isRandomPlayerOrder) { 
            Random.Shared.Shuffle(players); 
        }
        PlayManager = new PlayManager
        {
            Players = players.ToList(),
            IsSynchronousMode = isSynchronousMode
        };
        Boards = boardBuilders.Select(b => new Board(b)).ToList();
        ActionBuffer = [];
    }
    #endregion

    #region main data properties
    public PlayManager PlayManager {get;init;}
    public IReadOnlyList<Board> Boards {get;init;}
    public List<PlayAction> ActionBuffer {get;private set;}
    #endregion

    #region methods   
    public Board GetBoardByCode(int boardCode) => Boards[boardCode - 1];
    public Board GetBoardByIndex(int boardIndex) => Boards[boardIndex];

    public OneOf<NotFound, BoardIsDone, Result<int>> SelectBoard(int boardCode)
        => (boardCode <= 0 || boardCode > Boards.Count)
            ? new NotFound()
            : GetBoardByCode(boardCode).IsDone
            ? new BoardIsDone()
            : new Result<int>(boardCode - 1);

    /// <summary>
    /// Play a space by its space code.  Returns appropriate OneOf result.
    /// </summary>
    public OneOf<ActionQueuedSuccessfully, Result<char>, AlreadyPlayed, NotFound> PlaySpace(
        int boardIndex,
        int spaceCode,
        char player
    ) {
        var board = GetBoardByIndex(boardIndex);
        if (board.TryGetCoordinatesFromSpaceIndexCode(spaceCode, out var col, out var row)) {
            return PlaySpace(boardIndex, col, row, player)
                .Match<OneOf<ActionQueuedSuccessfully, Result<char>, AlreadyPlayed, NotFound>>( //have to provide return-type when going from OneOf to OneOf
                    success => success,
                    result => result,
                    alreadyPlayed => alreadyPlayed
                );
        } else {
            return new NotFound();
        }
    }
    
    /// <summary>
    /// Play a space by its coordinates.  Adds action to buffer if successful.
    /// </summary>
    public OneOf<ActionQueuedSuccessfully, Result<char>, AlreadyPlayed> PlaySpace(
        int boardIndex,
        int col,
        int row,
        char player
    ) {
        var board = Boards[boardIndex];
        var space = board.Spaces[col, row];
        
        // Check if already played by this player
        if (space.IsKnownToPlayer(player)) {
            return new AlreadyPlayed();
        }
        
        // Check if someone else already there
        if (space.MarkChar.HasValue) {
            space.MakeKnownToPlayer(player);
            return new Result<char>(space.MarkChar.Value);
        }
        
        // Add action to buffer (don't execute immediately in sync mode)
        ActionBuffer.Add(new PlayAction(boardIndex, col, row, player));
        
        return new ActionQueuedSuccessfully();
    }

    /// <summary>
    /// Execute all pending actions in the action buffer.
    /// Checks for collisions and places impasses where appropriate.
    /// </summary>
    public void ExecutePendingActions() {
        var actions = ActionBuffer.ToList();
        ActionBuffer.Clear();
        
        if (actions.Count == 0) return;
               
        foreach (var action in actions) {
            var board = GetBoardByIndex(action.BoardIndex);
            var space = board.Spaces[action.Col, action.Row];
            
            // Skip if board is done
            if (board.IsDone) {
                continue;
            }
            
            // Check for collision with opponent
            if (actions.Any(otherA => 
                otherA.BoardIndex == action.BoardIndex
                && otherA.Row == action.Row
                && otherA.Col == action.Col
                && otherA.Player != action.Player)
            ) {
                space.MarkChar = '█';
                foreach(var player in PlayManager.Players) {
                    space.MakeKnownToPlayer(player);    
                }
                continue;
            }
            
            // Execute successful play
            space.MarkChar = action.Player;
            space.MakeKnownToPlayer(action.Player);
        }
    }

    public void CheckIsValidToSave() {
        PlayManager.CheckIsValidToSave();
    }

    #endregion

    #region helper properties
    /// <summary>
    /// Returns a list of the active board indices. 0-based.
    /// </summary>
    [JsonIgnore()]
    public IEnumerable<int> ActiveBoardIndices {get {
        for(int i = 0; i < Boards.Count; i+=1) {
            if(!Boards[i].IsDone) {
                yield return i; 
            }
        }
    }}
    
    /// <summary>
    /// Returns null if there are 0 or multiple active boards. Board Index if there's 1.
    /// </summary>
    [JsonIgnore()]
    public int? SingleActiveBoardIndex {get {
        var firstElements = ActiveBoardIndices.Take(2).ToArray();
        return (firstElements.Length == 1)
            ? firstElements.Single()
            : null;
    }}

    /// <summary>
    /// Returns true if the game has ended, whether by tie or by winner.
    /// </summary>
    [JsonIgnore()]
    public bool IsGameOver
        => Boards.All(b => b.IsDone) || PlayManager.ActivePlayers.Count() == 1;
    
    /// <summary>
    /// Provides a short text summary of the current game-state. Particularly useful when the game is over.
    /// </summary>
    [JsonIgnore()]
    public string GameStateText
        => IsGameOver 
        ? (Winner.HasValue 
            ? $"Player '{Winner.Value}' wins!." 
            : "Tie game."
        ) 
        : PlayManager.GameStateText;
    
    [JsonIgnore()]
    /// <summary>
    /// Get the winner of the game.  Returns null if nobody has won yet or the
    /// game was a tie.  Note this is a heavy calculation and is not cached, but
    /// computers are fast.  TODO: Optimization.
    /// </summary>
    public char? Winner {
        get {
            if(!IsGameOver) {
                return null;
            }
            
            if(PlayManager.ActivePlayers.Count() == 1) {
                return PlayManager.ActivePlayers.Single();
            }
            var highestScore = ScoreCard.HighestScore;

            return highestScore.HasValue 
                ? highestScore.Value.Player 
                : null; // no winner found
        }
    }
    
    [JsonIgnore()]
    public ScoreCard ScoreCard 
        => Boards.Aggregate(
            new ScoreCard(),
            (prod, next) => prod + next.ScoreCard
            );
    #endregion
}

/// <summary>
/// Empty result struct for OneOf, used when a player tries to play on a space they already played.
/// </summary>
public struct AlreadyPlayed;

/// <summary>
/// Empty result struct for OneOf, used when the player tries to select a board that is done.
/// </summary>
public struct BoardIsDone;

/// <summary>
/// Action to be executed when playing a space.
/// </summary>
public record PlayAction(
    int BoardIndex,
    int Col,
    int Row,
    char Player
);

/// <summary>
/// Empty result struct for OneOf indicating success.
/// </summary>
public struct ActionQueuedSuccessfully;
