using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.TicTacToe;

namespace KriegspielTicTacToe.Model;
public static partial class GameTemplates {
    public static GameTemplate TicTacToe {get;} = new TicTacToeTemplate(
        "tictactoe",
        "Basic simple tic-tac-toe.",
        [
            new BoardBuilder(3, 3, new TicTacToeRuleset(IsBoardDoneWhenScored: true))
        ],
        isKriegspiel: false,
        isSynchronousMode: false
    );
}