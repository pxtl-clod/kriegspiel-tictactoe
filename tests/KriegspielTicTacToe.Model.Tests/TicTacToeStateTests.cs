namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class TicTacToeStateTests {
    [Fact]
    public void Constructor_EmptyBoards() {
        var state = new TicTacToeState(
            new[] { 'X', 'O' },
            Array.Empty<BoardBuilder>(),
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.Boards.Should().BeEmpty();
        state.PlayManager.Players.Should().ContainInOrder('X', 'O');
        state.PlayManager.ActivePlayers.Should().ContainInOrder('X', 'O');
    }

    [Fact]
    public void Constructor_WithBoardsCreatesProperState() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3), new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.Boards.Count.Should().Be(2);
        state.PlayManager.Players.Should().ContainInOrder('X', 'O');
        state.PlayManager.ActivePlayers.Should().ContainInOrder('X', 'O');
    }

    [Fact]
    public void Round_RoundIndexStartsAtZero() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.PlayManager.RoundIndex.Should().Be(0);
    }

    [Fact]
    public void EndTurn_AdvancesTurn() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.EndTurn('X', out _);
        state.PlayManager.ActivePlayers.Should().ContainInOrder('O');
    }

    [Fact]
    public void EndTurn_EndRound_TracksRoundIndex()
    {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        // X takes their turn first
        state.PlayManager.EndTurn('X', out _);
        // O is next
        state.PlayManager.EndTurn('O', out _);
        state.PlayManager.RoundIndex.Should().Be(0);
        state.PlayManager.IsRoundOver.Should().BeTrue();
        state.PlayManager.GameStateText.Should().Be("Round over.");
    }

    [Fact]
    public void RoundComplete_OnePlayerResigned() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('X');
        // X hasn't taken their turn because they're resigned
        state.PlayManager.EndTurn('O', out _); // O takes their turn, skipping X

        state.PlayManager.NumberOfActivePlayers.Should().Be(1);
        state.PlayManager.IsRoundOver.Should().BeTrue();
    }

    [Fact]
    public void RoundComplete_OnePlayerLeft_Resigns() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.EndTurn('X', out _);
        state.PlayManager.EndTurn('O', out _);
        state.PlayManager.IsRoundOver.Should().BeTrue();
    }

    [Fact]
    public void RoundComplete_ThreePlayers() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.NumberOfActivePlayers.Should().Be(3);

        state.PlayManager.EndTurn('A', out _);
        state.PlayManager.EndTurn('B', out _);
        state.PlayManager.EndTurn('C', out _);
        state.PlayManager.RoundIndex.Should().Be(0);
        state.PlayManager.IsRoundOver.Should().BeTrue();
    }

    [Fact]
    public void ActivePlayers_OnlyLivePlayers() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        state.PlayManager.ActivePlayers.Should().ContainInOrder('B', 'C');
    }

    [Fact]
    public void SingleActivePlayer_GameOver() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        state.PlayManager.ResignPlayer('B');
        state.PlayManager.NumberOfActivePlayers.Should().Be(1);
        
        state.PlayManager.EndTurn('C', out _);
        state.PlayManager.IsRoundOver.Should().BeTrue();
    }

    [Fact]
    public void GameStateText_SinglePlayerLeft() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        state.PlayManager.ResignPlayer('B');
        state.PlayManager.EndTurn('C', out _);
        state.PlayManager.IsRoundOver.Should().BeTrue();
        state.PlayManager.GameStateText.Should().Be("Round over.");
    }

    [Fact]
    public void GameStateText_SecondsToGo() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        var text = state.PlayManager.GameStateText;
        text.Should().Contain("Player B");
    }

    [Fact]
    public void Constructor_3Players_CorrectActiveCount() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ActivePlayers.First().Should().Be('A');
    }

    [Fact]
    public void Constructor_RandomPlayer_Shuffles() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: true,
            isSynchronousMode: false
        );
        var firstPlayer = state.PlayManager.ActivePlayers.First();
        var expectedPlayers = new[] { 'A', 'B', 'C' };
        expectedPlayers.Contains(firstPlayer).Should().BeTrue();
    }

    [Fact]
    public void CanTakeTurn_OnlyActivePlayers() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        
        // A has resigned, so A should be unable to take turns
        state.PlayManager.CanTakeTurn('A').Should().BeFalse();
        
        // B is next to take a turn
        state.PlayManager.CanTakeTurn('B').Should().BeTrue();
        state.PlayManager.CanTakeTurn('C').Should().BeFalse();
        state.PlayManager.CanTakeTurn('D').Should().BeFalse();
    }

    [Fact]
    public void ResignPlayer_RemovesFromActive() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        state.PlayManager.ResignedPlayersSet.Should().Contain('A');
        state.PlayManager.ActivePlayers.Should().NotContain('A');
    }

    [Fact]
    public void ResignPlayer_SkipsResignedTurn() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        state.PlayManager.ResignPlayer('A');
        state.PlayManager.EndTurn('A', out _); // A was resigned, their turn was skipped

        state.PlayManager.ActivePlayers.First().Should().Be('B');
        state.PlayManager.EndTurn('B', out _);
        state.PlayManager.EndTurn('C', out _);
    }
}
