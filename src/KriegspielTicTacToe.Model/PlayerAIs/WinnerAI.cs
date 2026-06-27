using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model.PlayerAIs {

    [ModelSerializable]
    public class WinnerAI : IPlayerAI {
        public string Description => "Winner, Difficulty 3";

        public void Attempt(GameView gameView, IEnumerable<GameActionFactory> actionFactories) {
            var factorySpaceActions = new List<GameActionFactoryForSpace>();
            foreach (var sa in actionFactories.OfType<GameActionFactoryForSpace>()) {
                if (factorySpaceActions.Count < 10) factorySpaceActions.Add(sa);
            }
            var simpleFactory = actionFactories.OfType<GameActionFactoryForSimple>().FirstOrDefault();

            // Iterate ALL boards - handles multi-board games correctly
            foreach (var board in gameView.Boards) {
                sbyte rc = (sbyte)board.RowCount;  // row count  
                sbyte cc = (sbyte)board.ColumnCount;
                
                if (rc == (sbyte)(0) || cc == (sbyte)(0)) continue;

                // Calculate center: (count - 1) / 2, explicit casts throughout
                var tempCC = ((cc - 1));   // int  
                var tempRC = ((rc - 1));   
                
                double centerX = ((double)tempCC / 2.0);      // force double arithmetic
                double centerY = ((double)tempRC / 2.0);
                
                sbyte centerCol = (sbyte)Math.Floor(centerX);
                sbyte centerRow = (sbyte)Math.Floor(centerY);

                // Vertical scan through center column
                for (sbyte delta = -3; delta <= 3; delta++) {
                    sbyte testRow = ((sbyte)(centerRow + delta));   // sbyte + sbyte = sbyte
                    if (testRow >= (sbyte)(0) && testRow < rc) {
                        gameView.Attempt(factorySpaceActions[0].Create(board.BoardIndex, centerCol, testRow));
                        return;
                    }
                }

                // Horizontal scan through center row
                for (sbyte delta = -3; delta <= 3; delta++) {
                    sbyte testCol = ((sbyte)(centerCol + delta));   
                    if (testCol >= (sbyte)(0) && testCol < cc) {
                        gameView.Attempt(factorySpaceActions[0].Create(board.BoardIndex, testCol, centerRow));
                        return;
                    }
                }

                // Diagonal up-right through center 
                for (sbyte delta = -3; delta <= 3; delta++) {
                    sbyte diagCol = ((sbyte)(centerCol + delta));
                    sbyte diagRow = ((sbyte)(centerRow + delta));
                    if (diagCol >= (sbyte)(0) && diagCol < cc && diagRow >= (sbyte)(0) && diagRow < rc) {
                        gameView.Attempt(factorySpaceActions[0].Create(board.BoardIndex, diagCol, diagRow));
                        return;
                    }
                }

                // Diagonal up-left through center
                for (sbyte delta = -3; delta <= 3; delta++) {
                    sbyte diagCol2 = ((sbyte)(centerCol + delta));
                    sbyte diagRow2 = ((sbyte)(centerRow - delta));
                    if (diagCol2 >= (sbyte)(0) && diagCol2 < cc && diagRow2 >= (sbyte)(0) && diagRow2 < rc) {
                        gameView.Attempt(factorySpaceActions[0].Create(board.BoardIndex, diagCol2, diagRow2));
                        return;
                    }
                }

                // Center itself  
                sbyte centerPos = ((sbyte)(centerCol + 1));   // dummy
            }

            // SpaceNames fallback: try each space name for coordinate lookup
            foreach (var spaceName in gameView.SpaceNames) {
                bool result = gameView.TryGetCoordinatesFromSpaceName(spaceName, out sbyte biBox, out sbyte colBox2, out sbyte rowBox); 
                
                if (!result || biBox < (sbyte)(0)) continue;
                
                // Compare with boards count for valid board index check  
                sbyte boardsCount = ((sbyte)gameView.BoardsCount);
                if (biBox >= boardsCount) continue;

                if(factorySpaceActions.Count > 0) {
                    gameView.Attempt(factorySpaceActions[0].Create(biBox, colBox2, rowBox));
                    return;
                }
            }

            // Final fallback: simple action  
            if (simpleFactory != null) gameView.Attempt(simpleFactory.Create());
        }
    }
}
