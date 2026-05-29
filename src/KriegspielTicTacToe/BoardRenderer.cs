namespace KriegspielTicTacToe;

using KriegspielTicTacToe.Model;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Draws the full board based on the given gamestate, from the perspective of
/// the given player.
/// </summary>
public static class BoardRenderer {
    private static StringBuilder sb = new StringBuilder();

    /// <summary>
    /// Draws the full boards based on the state and returns a string-builder.
    /// </summary>
    public static StringBuilder DrawBoards(TicTacToeState state, Player player, int? activeBoardIndex) {
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

    /// <summary>
    /// Helper function to draw a border row of the board.
    /// </summary>
    private static int DrawBorderRow(
        TicTacToeState state,
        int startBoardIndex,
        string startBarString, 
        string midBarString, 
        string endBarString, 
        string spanString, 
        bool showBoardCode,
        StringBuilder sb)
    {
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

    /// <summary>
    /// Draw a row of board spaces.
    /// </summary>
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

    /// <summary>
    /// Renders the entire board as a string.
    /// </summary>
    public static string Render(TicTacToeState state, Player player, int? activeBoardIndex) {
        return DrawBoards(state, player, activeBoardIndex).ToString();
    }
}
