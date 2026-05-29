namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using KriegspielTicTacToe.Model;
using System.Text;
using Xunit;

/// <summary>
/// Tests for the full Board render output structure.
/// </summary>
public class BoardRendererFullRenderTests {
    
    [Fact]
    public void FullBoardRender_OutputStructure() {
        // Create 2 boards of 3x3 and 1 board of 4x3 using BoardBuilder pattern
        var board1 = new BoardBuilder(3, 3);
        var board2 = new BoardBuilder(3, 3);
        var board3 = new BoardBuilder(4, 3);
        
        var state = new TicTacToeState(
            new[] { 'X', 'O' },
            new[] { board1, board2, board3 },
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        // Each player needs to make moves on each board
        var oPlayer = new Player("O");
        
        // Simulate some turns
        state.PlayManager.EndTurn(new Player("X"), out _);
        state.PlayManager.EndTurn(oPlayer, out _);
        
        // Get active board index or null if multiple boards active
        var activeIndex = state.SingleActiveBoardIndex;
        
        // Create a StringBuilder and draw boards
        var sb = new StringBuilder();
        
        // Call overloaded DrawBoards that takes StringBuilder and fills in empty boards
        BoardRenderer.DrawBoards(state, oPlayer, activeIndex, sb);
        
        var result = sb.ToString();
        
        // The result should contain the board numbers and borders
        result.Should().Contain("1");
        result.Should().Contain("2");
        result.Should().Contain("3");
        result.Should().Contain("┌─");
        result.Should().Contain("└───┴───┴───┘");
    }
}
