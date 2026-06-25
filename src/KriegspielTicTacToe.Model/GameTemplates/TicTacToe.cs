using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.MNKGame;

namespace KriegspielTicTacToe.Model;
public static partial class GameTemplates {
    public static GameTemplate TicTacToe {get;} = new MNKTemplate(
        "tictactoe",
        "Basic simple tic-tac-toe.",
        [
            new BoardBuilder(3, 3, new MNKRuleset(IsBoardDoneWhenScored: true))
        ],
        isKriegspiel: false,
        isSynchronousMode: false
    );
}