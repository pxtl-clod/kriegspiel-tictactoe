namespace KriegspielTicTacToe.Model;

public abstract record PlayManagerFactory() {
    public abstract PlayManager Create(IReadOnlyList<Player> players);
}
