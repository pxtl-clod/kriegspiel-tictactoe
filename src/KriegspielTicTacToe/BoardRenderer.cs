using KriegspielTicTacToe.Model;
using System.Text;

public static class BoardRenderer {
    public static StringBuilder DrawBoards(TicTacToeState state, Player player, int? activeBoardIndex, StringBuilder sb = null) {
        sb ??= new StringBuilder();
        bool doShowBoardCode = state.Boards.Count > 1;
        var maxRowCount = state.Boards.Max(b => b.RowCount);

        var nextDrawnBoardIndex = 0;
        while (nextDrawnBoardIndex < state.Boards.Count) {
            DrawBorderRow(state, nextDrawnBoardIndex, "┌", "┬", "┐", "───", doShowBoardCode, sb);
            
            for(var row = 0; row < maxRowCount; row+=1) {
                if(row > 0) {
                    DrawBorderRow(state, nextDrawnBoardIndex, "├", "┼", "┤", "───", false, sb);
                }
                DrawBoardSpacesRow(state, nextDrawnBoardIndex, player, "│", activeBoardIndex, row, sb);
            }
            nextDrawnBoardIndex = DrawBorderRow(state, nextDrawnBoardIndex, "└", "┴", "┘", "───", false, sb);
            sb.AppendLine();
        }
        if(state.PlayManager.ResignedPlayersSet.Count > 0) {
            foreach(var resignedPlayer in state.PlayManager.ResignedPlayersSet) {
                sb.AppendLine($" - player '{resignedPlayer}' resigned.");
            }
        }

        return sb;
    }

    public static int GetBoardDrawWidth(Board board) 
        => board.ColumnCount * 4 + 3;

    private static int DrawBorderRow(TicTacToeState state, int startBoardIndex, string startBarString, string midBarString, string endBarString, string spanString, bool showBoardCode, StringBuilder sb) {
        for (var boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex+=1) {
            var board = state.Boards[boardIndex];
            
            sb.Append(showBoardCode
                ? (board.IsDone ? " ✓" : $" {boardIndex + 1}")
                : "  ");

            sb.Append($"{startBarString}{spanString}");
            
            for(var col = 0; col < board.ColumnCount-1; col+=1) {
                sb.Append($"{midBarString}{spanString}");
            }
            sb.Append(endBarString);
        }
        sb.AppendLine();
        return state.Boards.Count;
    }

    private static int DrawBoardSpacesRow(TicTacToeState state, int startBoardIndex, Player player, string borderBarString, int? activeBoardIndex, int rowIndex, StringBuilder sb) {
        for (int boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex+=1) {
            var board = state.Boards[boardIndex];

            sb.Append("  ");

            for(var col = 0; col < board.ColumnCount; col+=1) {
                var body = ModelToKeyUtility.GetSpaceString(state, player, boardIndex, activeBoardIndex, col, rowIndex);
                body = body.PadLeft(2);
                body = body.PadRight(3);
                sb.Append($"{borderBarString}{body}");
            }
            sb.Append(borderBarString);
        }
        sb.AppendLine();
        return state.Boards.Count;
    }

    public static string Render(TicTacToeState state, Player player, int? activeBoardIndex) {
        // Get the current string representation from DrawBoards which handles empty boards
        return DrawBoards(state, player, activeBoardIndex).ToString();
    }
}

/// <summary>
/// Test utility to get string for a space.
/// </summary>
public static class ModelToKeyUtility {
    /// <summary>
    /// Gets the string representation for a space given the board state.
    /// </summary>
    /// <param name="state">The current game state</param>
    /// <param name="player">The observing player</param>
    /// <param name="boardIndex">The board index</param>
    /// <param name="activeBoardIndex">The active board index (null if rendering all)</param>
    /// <param name="col">Column index</param>
    /// <param name="row">Row index</param>
    /// <returns>String representation for the space</returns>
    public static string GetSpaceString(TicTacToeState state, Player player, int boardIndex, int? activeBoardIndex, int col, int row) {
        // If active board index is specified and different from this board, the space is unknown
        bool boardActive = activeBoardIndex == boardIndex;
        
        if(!boardActive) return "   ";
        
        var space = state.Boards[boardIndex].Spaces[col, row];
        
        // If space has a mark, show it
        if(space.Mark != null) return space.Mark.ToString();
        
        // Otherwise show space name or empty
        return "   ";
    }
}
