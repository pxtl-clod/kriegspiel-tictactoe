namespace KriegspielTicTacToe.Model;

public class PlayActionBuffer {
    public List<TicTacToePlayAction> Actions {get;private set;} = [];

    public void Add(TicTacToePlayAction action) {
        Actions.Add(action);
    }

    public void Clear() {
        Actions.Clear();
    }

    /// <summary>
    /// Execute all pending actions in the action buffer.
    /// Checks for collisions and places impasses where appropriate.
    /// </summary>
    public void ExecutePendingActions() {
        if (Actions.Count == 0) return;
               
        foreach (var action in Actions) {
            var board = GameState!.GetBoardByIndex(action.BoardIndex);
            var space = board.Spaces[action.Col, action.Row];
            
            // Skip if board is done
            if (board.IsDone) {
                continue;
            }
            
            // Check for collision with opponent
            if (Actions.Any(otherA => 
                otherA.BoardIndex == action.BoardIndex
                && otherA.Row == action.Row
                && otherA.Col == action.Col
                && otherA.Player != action.Player)
            ) {
                space.MarkChar = '█';
                foreach(var player in GameState.PlayManager.Players) {
                    space.MakeKnownToPlayer(player);    
                }
                continue;
            }
            
            // Execute successful play
            space.MarkChar = action.Player;
            space.MakeKnownToPlayer(action.Player);
        }

        Clear();
    }

    /// <summary>
    /// The game state.  Is set by parent's Init, but Init is
    /// post-constructor so must be nullable.
    /// </summary>
    [JsonIgnore()]
    public TicTacToeState? GameState { get; internal set; }
}