namespace KriegspielTicTacToe.Model;

public interface IGameState {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    PlayManager PlayManager {get;}
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    IReadOnlyList<Board> Boards {get;}
}
