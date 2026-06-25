using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.TicTacToe;

namespace KriegspielTicTacToe.Model;
public static partial class GameTemplates {
    public static GameTemplate KriegspielTicTacToe {get;} = new TicTacToeTemplate(
        "kriegspiel-tictactoe",
        "Zach Weinersmith's Kriegspiel Tic-Tac-Toe.",
        [
            new BoardBuilder(3, 3, new TicTacToeRuleset()),
            new BoardBuilder(3, 3, new TicTacToeRuleset()),
            new BoardBuilder(3, 3, new TicTacToeRuleset())
        ],
        isKriegspiel: true,
        isSynchronousMode: false
    );
}