namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using KriegspielTicTacToe.Model;
using Xunit;

#pragma warning disable IDE0027 // Simplify nested expression
public class BoardRendererTests {
    
    [Fact]
    public void Constructor_3x3_EmptyBoard_DoesNotCrash() {
        var board = new Board(3, 3);
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[0, 0].MakeKnownToPlayer("O");
        board.ScoreCard.HighestScore.Should().BeNull();
    }

    [Fact]
    public void Constructor_FullBoard_ScoreWorks() {
        var board = new Board(3, 3);
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[1, 0].Mark = "X";
        board.Spaces[2, 0].Mark = "X";
        
        var result = board.ScoreCard.HighestScore;
        result.Should().NotBeNull();
        if(result.HasValue)
            result.Value.Player.Mark.Should().Be("X");
    }

    [Fact]
    public void DrawBoards_3x3_ReturnsString() {
        var boardBuilder = new BoardBuilder(3, 3);
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        var currentPlayer = state.PlayManager.PlayersAvailableForTurn.First();
        
        var str = BoardRenderer.Render(state, currentPlayer, activeBoardIndex: null);
        
        str.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DrawBoards_WithOneMove_DoesNotScore() {
        var boardBuilder = new BoardBuilder(3, 3);
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("X"), out _);
        
        state.Boards[0].ScoreCard.HighestScore.Should().BeNull();
    }

    [Fact]
    public void DrawBoards_FullHorizontalLine_XWins() {
        var boardBuilder1 = new BoardBuilder(3, 3);
        var boardBuilder2 = new BoardBuilder(3, 3);
        var board2 = new Board(boardBuilder2);
        board2.Spaces[0, 0].Mark = "X"; 
        board2.Spaces[1, 0].Mark = "X"; 
        board2.Spaces[2, 0].Mark = "X";
        
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder1, boardBuilder2 }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("X"), out _);
        
        board2.ScoreCard.HighestScore?.Player.Mark.Should().Be("X");
    }

    [Fact]
    public void DrawBoards_FullVerticalLine_OWins() {
        var boardBuilder1 = new BoardBuilder(3, 3);
        var boardBuilder2 = new BoardBuilder(3, 3);
        var board2 = new Board(boardBuilder2);
        board2.Spaces[0, 0].Mark = "O"; 
        board2.Spaces[0, 1].Mark = "O"; 
        board2.Spaces[0, 2].Mark = "O";
        
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder1, boardBuilder2 }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("O"), out _);
        
        board2.ScoreCard.HighestScore?.Player.Mark.Should().Be("O");
    }

    [Fact]
    public void DrawBoards_FullDiagonal_XWins_Identity() {
        var boardBuilder1 = new BoardBuilder(3, 3);
        var boardBuilder2 = new BoardBuilder(3, 3);
        var board2 = new Board(boardBuilder2);
        board2.Spaces[0, 0].Mark = "X"; 
        board2.Spaces[1, 1].Mark = "X"; 
        board2.Spaces[2, 2].Mark = "X";
        
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder1, boardBuilder2 }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("X"), out _);
        
        board2.ScoreCard.HighestScore?.Player.Mark.Should().Be("X");
    }

    [Fact]
    public void DrawBoards_FullDiagonal_OWins_Inverse() {
        var boardBuilder1 = new BoardBuilder(3, 3);
        var boardBuilder2 = new BoardBuilder(3, 3);
        var board2 = new Board(boardBuilder2);
        board2.Spaces[0, 2].Mark = "O"; 
        board2.Spaces[1, 1].Mark = "O"; 
        board2.Spaces[2, 0].Mark = "O";
        
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder1, boardBuilder2 }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("O"), out _);
        
        board2.ScoreCard.HighestScore?.Player.Mark.Should().Be("O");
    }

    [Fact]
    public void DrawBoards_MultipleBoards_HandlesMultipleBoards() {
        var boardBuilder1 = new BoardBuilder(3, 3);
        var boardBuilder2 = new BoardBuilder(3, 3);
        
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder1, boardBuilder2 }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        state.PlayManager.EndTurn(new Player("X"), out _);
        state.PlayManager.EndTurn(new Player("O"), out _);
        
        var str = BoardRenderer.Render(state, new Player("X"), activeBoardIndex: null);
        
        str.Should().NotBeNullOrEmpty();
    }
}
#pragma warning restore IDE0027 // Simplify nested expression
