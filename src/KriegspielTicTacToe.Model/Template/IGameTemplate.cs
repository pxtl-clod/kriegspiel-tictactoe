namespace KriegspielTicTacToe.Model.Template;

public interface IGameTemplate {
    PlayManagerFactory PlayManagerFactory { get;}
    
    IReadOnlyList<Board> ConstructBoards();
}
