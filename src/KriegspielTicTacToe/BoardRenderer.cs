namespace KriegspielTicTacToe;

using KriegspielTicTacToe.Model;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Draws the full board based on the given gamestate, from the perspective of
/// the given player.
/// </summary>
public static class BoardRenderer {
    /// <summary>
    /// Draws the full board based on the given gamestate, from the perspective of
    /// the given player.
    /// </summary>
    public static StringBuilder DrawBoards(TicTacToeState state, Player player, int? activeBoardIndex) {
        bool doShowBoardCode = state.Boards.Count > 1;
        var maxRowCount = state.Boards.Max(b => b.RowCount);
        var sb = new StringBuilder();

        var nextDrawnBoardIndex = 0;
        while (nextDrawnBoardIndex < state.Boards.Count) {
            //top row border
            sb.Append(DrawBorderRow(state, nextDrawnBoardIndex, "┌", "┬", "┐", "───", doShowBoardCode));
            
            for(var row = 0; row < maxRowCount; row+=1) {
                if(row > 0) {
                    //internal border
                    DrawBorderRow(state, nextDrawnBoardIndex, "├", "┼", "┤", "───", showBoardCode: false, sb);
                }
                DrawBoardSpacesRow(state, nextDrawnBoardIndex, player, "│", activeBoardIndex, row, sb);
            }
            nextDrawnBoardIndex = DrawBorderRow(state, nextDrawnBoardIndex, "└", "┴", "┘", "───", showBoardCode: false, sb);
            sb.AppendLine();
        }
        if(state.PlayManager.ResignedPlayersSet.Count > 0) {
            foreach(var resignedPlayer in state.PlayManager.ResignedPlayersSet) {
                sb.AppendLine($" - player '{resignedPlayer}' is resigned.");
            }
        }

        return sb;
    }

    public static int GetBoardDrawWidth(Board board) 
        => board.ColumnCount * 4 + 3;

    /// <summary>
    /// Helper function to draw a border row of the board.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to. If null, returns the result as string.</param>
    /// <returns>
    /// Returns the index of the next board to draw if it needs another row of
    /// boards.
    /// </returns>
    private static int DrawBorderRow(
        TicTacToeState state,
        int startBoardIndex,
        string startBarString, 
        string midBarString, 
        string endBarString, 
        string spanString, 
        bool showBoardCode,
        StringBuilder? sb = null)
    {
        for (var boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex+=1) {
            var board = state.Boards[boardIndex];
            (var cursorLeft, var cusorTop) = Console.GetCursorPosition();
            if(cursorLeft > 0 && (cursorLeft + GetBoardDrawWidth(board) > Console.WindowWidth)) {
                sb?.AppendLine();
                return boardIndex;
            }

            Console.Out.Write(showBoardCode
                ? (board.IsDone
                    ? " ✓" //board is done so just show a checkmark.
                    : $" {boardIndex + 1}" //key-index to choose it
                ): "  " //blank space
            );

            sb?.Append(showBoardCode
                ? (board.IsDone
                    ? " ✓" //board is done so just show a checkmark.
                    : $" {boardIndex + 1}" //key-index to choose it
                ): "  " //blank space
            );
            Console.Out.Write($"{startBarString}{spanString}");
            sb?.Append($"{startBarString}{spanString}");
            
            for(var col = 0; col < board.ColumnCount-1; col+=1) {
                Console.Out.Write($"{midBarString}{spanString}");
                sb?.Append($"{midBarString}{spanString}");
            }
            Console.Out.Write(endBarString);
            sb?.Append(endBarString);
        }
        Console.Out.WriteLine();
        sb?.AppendLine();
        return state.Boards.Count;
    }

    /// <summary>
    /// Draw a row of board spaces.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to. If null, returns the result as string.</param>
    /// <returns>
    /// Returns the index of the next board to draw if it needs another row of
    /// boards.
    /// </returns>
    private static int DrawBoardSpacesRow(TicTacToeState state, int startBoardIndex, Player player, string borderBarString, int? activeBoardIndex, int rowIndex, StringBuilder? sb = null) {
        for (int boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex+=1) {
            var board = state.Boards[boardIndex];
            (var cursorLeft, var cusorTop) = Console.GetCursorPosition();
            if(cursorLeft > 0 && (cursorLeft + GetBoardDrawWidth(board) > Console.WindowWidth)) {
                Console.Out.WriteLine();
                sb?.AppendLine();
                return boardIndex;
            }

            Console.Out.Write("  ");
            sb?.Append("  ");

            for(var col = 0; col < board.ColumnCount; col+=1) {
                DrawSpaceBody(
                    ModelToKeyUtility.GetSpaceString(state, player, boardIndex, activeBoardIndex, col, rowIndex), 
                    borderBarString,
                    sb);
            }
            Console.Out.Write(borderBarString);
            sb?.Append(borderBarString);
        }
        Console.Out.WriteLine();
        sb?.AppendLine();
        return state.Boards.Count();
    }

    /// <summary>
    /// Helper function to draw the body-spaces of the board.
    /// </summary>
    /// <param name="body">The space body string</param>
    /// <param name="borderBarString">The border bar string</param>
    /// <param name="sb">Optional StringBuilder to append to</param>
    private static void DrawSpaceBody(string body, string borderBarString, StringBuilder? sb = null)
    {
        body = body.PadLeft(2);
        body = body.PadRight(3);
        Console.Out.Write($"{borderBarString}{body}");
        sb?.Append($"{borderBarString}{body}");
    }

    /// <summary>
    /// Renders the entire board as a string.
    /// </summary>
    public static string Render(TicTacToeState state, Player player, int? activeBoardIndex) {
        var sb = DrawBoards(state, player, activeBoardIndex);
        return sb.ToString();
    }
}
