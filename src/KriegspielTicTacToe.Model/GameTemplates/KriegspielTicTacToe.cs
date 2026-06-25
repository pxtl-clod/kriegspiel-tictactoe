using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.MNKGame;

namespace KriegspielTicTacToe.Model;
public static partial class GameTemplates {
    public static GameTemplate KriegspielTicTacToe {get;} = new MNKTemplate(
        "kriegspiel-tictactoe",
        "Zach Weinersmith's Kriegspiel Tic-Tac-Toe.",
        [
            new BoardBuilder(3, 3, new MNKRuleset()),
            new BoardBuilder(3, 3, new MNKRuleset()),
            new BoardBuilder(3, 3, new MNKRuleset())
        ],
        isKriegspiel: true,
        isSynchronousMode: false
    );
}