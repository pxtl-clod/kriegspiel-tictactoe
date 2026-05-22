namespace KriegspielTicTacToe.Model;

/// <summary>
/// Represents a player in the game. Stores their marker character.
/// </summary>
public record Player(char Value) {
    /// <summary>
    /// Create a Player from a string.
    /// </summary>
    public static Player FromString(string value) => new(value[0]);
    
    /// <summary>
    /// Implicit conversion from string to Player.
    /// </summary>
    public static implicit operator Player(string value) => new(value[0]);
    
    /// <summary>
    /// Implicit conversion from char to Player.
    /// </summary>
    public static implicit operator Player(char value) => new(value);
    
    /// <summary>
    /// Implicit conversion from Player to char.
    /// </summary>
    public static implicit operator char(Player value) => value.Value;
}
