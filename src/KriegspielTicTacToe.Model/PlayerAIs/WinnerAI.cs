using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model.PlayerAIs;

[ModelSerializable]
public class WinnerAI : IPlayerAI {
    public string Description => "Winner, Difficulty 3";

    /// <summary>
    /// Attempt a move using smart positions on all boards. Handles 3x3 and larger boards correctly.
    /// Uses coordinates (col, row) directly instead of space names.
    /// </summary>
    public void Attempt(GameView gameView, IEnumerable<GameActionFactory> actionFactories)
    {
        var spaceActions = actionFactories.OfType<GameActionFactoryForSpace>().ToList();
        var simpleActions = actionFactories.OfType<GameActionFactoryForSimple>().ToList();

        // First: try smart coordinate-based positions on 3x3 boards only
        foreach (var board in gameView.Boards)
        {
            if (board.ColumnCount != 3 || board.RowCount != 3) continue;

            // Smart position priority using explicit coordinates:
            // (col, row) are zero-based integers for a 3x3 board
            var smartPositions = new[] { 
                ((sbyte)1, (sbyte)1),   // center - highest priority
                ((sbyte)0, (sbyte)0),   // bottom-left corner
                ((sbyte)2, (sbyte)0),   // top-right corner  
                ((sbyte)0, (sbyte)2),   // top-left corner
                ((sbyte)2, (sbyte)2),   // bottom-right corner
            };

            foreach (var pos in smartPositions)
            {
                if (spaceActions.Count > 0)
                {
                    var result = gameView.Attempt(spaceActions[0].Create(board.BoardIndex, pos.Item1, pos.Item2));
                    if (result.IsTurnDone) {
                        return;
                    }
                }
            }
        }

        // Second: for larger boards or when no space moves were available, try SpaceNames fallback
        foreach (var spaceName in gameView.SpaceNames)
        {
            if (gameView.TryGetCoordinatesFromSpaceName(spaceName, out sbyte boardIndex, out var col, out var row))
            {
                // Only try space actions on boards where they're available.
                if (spaceActions.Count > 0)
                {
                    var result = gameView.Attempt(spaceActions[0].Create(boardIndex, col, row));
                    if (result.IsTurnDone) {
                        return;
                    }
                }
            }
        }

        // Space actions not available for this game, fallback to simple actions.
        if (simpleActions.Count > 0)
        {
            var simple = simpleActions[simpleActions.Count - 1];
            var result = gameView.Attempt(simple.Create());
            if (result.IsTurnDone) {
                return;
            }
        }
    }
}
