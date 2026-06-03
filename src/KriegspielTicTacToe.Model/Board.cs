namespace KriegspielTicTacToe.Model;

using System.IO;
using OneOf;

/// <summary>
/// JSON-serializable model object for a single tic-tac-toe board.
/// </summary>
public record Board {
    #region constructors
    public Board() {}
    public Board(BoardBuilder builder) : this(builder.Width, builder.Height) {}
    public Board(byte width, byte height) {
        Spaces = new Space[width, height];
        for (var col = 0; col < Spaces.GetLength(0); col+=1) {
            for (var row = 0; row < Spaces.GetLength(1); row+=1) {
                Spaces[col,row] = new Space();
            }
        }
    }
    #endregion

    #region main data properties
    public Space[,] Spaces {get;set;} = new Space[1,1]{{new Space()}}; //default value is dummy board, never use.
    #endregion

    #region Methods
    /// <summary>
    /// For the given space on the board, generate the space's name.
    /// </summary>
    public int GetSpaceNameAsInt(int col, int row) {
        //aims for basic 3x3, but larger if needed
        //7 8 9
        //4 5 6
        //1 2 3
        return Spaces.GetLength(1) * (Spaces.GetLength(0) - 1) //top-left
            + col
            - row * Spaces.GetLength(0)
            + 1; //1-based
    }

    /// <summary>
    /// For the given space index code, find the coordinates.  Uses a "Try"
    /// signature so that it shall return false if the given spaceindex is not
    /// on the board at all.
    /// </summary>
    public bool TryGetCoordinatesFromSpaceNameAsInt(int spaceName, out int resultCol, out int resultRow) {
        //brute-force search
        //todo: smarter algo
        for (var col = 0; col < Spaces.GetLength(0); col+=1) {
            for (var row = 0; row < Spaces.GetLength(1); row+=1) {
                if(GetSpaceNameAsInt(col, row) == spaceName) {
                    resultCol = col;
                    resultRow = row;
                    return true;
                }
            }
        }
        resultCol = resultRow = -1;
        return false;
    }
        
    /// <summary>
    /// Get all of the spaces on the board as a big enumerable that you can
    /// foreach across.
    /// </summary>
    public System.Collections.Generic.IEnumerable<Space> BoardAsEnumerable() {
        foreach(var space in Spaces) {
            yield return space;    
        }
    }
    #endregion

    #region helper properties
    [JsonIgnore()]
    public int ColumnCount
        => Spaces.GetLength(0);

    [JsonIgnore()]    
    public int RowCount
        => Spaces.GetLength(1);
        
    /// <summary>
    /// Get how many spaces are on the board.
    /// </summary>
    [JsonIgnore()]
    public int SpaceCount
        => Spaces.GetLength(0) * Spaces.GetLength(1);

    /// <summary>
    /// Get how many digits the users will have to type in to type in a
    /// space-name.
    /// </summary>
    [JsonIgnore()]
    public int SpaceNameLength
        => SpaceCount.ToString().Length;

    /// <summary>
    /// Returns true if the game has ended in a tie.
    /// </summary>
    [JsonIgnore()]
    public bool IsFull 
        => BoardAsEnumerable().All(s => s.Mark != null);

    [JsonIgnore()]
    public bool IsDone
        => IsFull 
        || ScoreCard.HighestScore.HasValue;

    [JsonIgnore()]
    public ScoreCard ScoreCard {
        get {
            var result = new System.Collections.Generic.List<PlayerScore>();
            var width = Spaces.GetLength(0);
            var height = Spaces.GetLength(1);
            var horizScoreLen = Spaces.GetLength(0);
            var vertScoreLen = Spaces.GetLength(1);
            var diagScoreLen = (int)System.Math.Min(horizScoreLen, vertScoreLen);
            
            //search for full-rows (left-to-right only)
            for(int row = 0; row < vertScoreLen; row+=1) {
                bool isWinner = true;
                string? lineOwner = Spaces[0,row].Mark;
                if(lineOwner != null) {
                    for(int col = 1; col < width && col < horizScoreLen; col+=1) {
                        if(lineOwner != Spaces[col,row].Mark) {
                            isWinner = false;
                            break;
                        }
                    }
                    if(isWinner) {
                        result.Add(new PlayerScore(new Player(lineOwner), 1));
                    }
                }
            }
            //search for full-columns (top-to-bottom only)
            // each column is checked exactly once as a vertical line
            for(int col = 0; col < width; col+=1) {
                bool isWinner = true;
                string? lineOwner = Spaces[col,0].Mark;
                if(lineOwner != null) {
                    for(int row = 1; row < height && row < vertScoreLen; row+=1) {
                        if(lineOwner != Spaces[col,row].Mark) {
                            isWinner = false;
                            break;
                        }
                    }
                    if(isWinner) {
                        result.Add(new PlayerScore(new Player(lineOwner), 1));
                    }
                }
            }
            // Check identity diagonals (going down-right, slope +1)
            // Starting columns: 0 to width - diagLen
            // Starting rows: 0 to height - diagLen
            for(int startCol = 0; startCol <= horizScoreLen - diagScoreLen; startCol+=1) {
                for(int startRow = 0; startRow <= vertScoreLen - diagScoreLen; startRow+=1) {
                    bool isWinner = true;
                    string? lineOwner = Spaces[startCol, startRow].Mark;
                    if(lineOwner != null) {
                        for(int d = 1; d < diagScoreLen; d+=1) {
                            if(lineOwner != Spaces[startCol + d, startRow + d].Mark) {
                                isWinner = false;
                                break;
                            }
                        }
                        if(isWinner) {
                            result.Add(new PlayerScore(new Player(lineOwner), 1));
                        }
                    }
                }
            }
            // Check inverse diagonals (going up-right, slope -1: row decreases)
            // A starting position (startCol, startRow) with row decreasing needs:
            //   - diagLen cells starting at startCol, moving +1 col and -1 row
            //   - final position: (startCol + diagLen - 1, startRow - diagLen + 1)
            //   - bounds: startCol + diagLen - 1 < width  AND  startRow - diagLen + 1 >= 0
            //   - So: startCol <= width - diagLen  AND  startRow >= diagLen - 1
            for(int startCol = 0; startCol <= horizScoreLen - diagScoreLen; startCol+=1) {
                for(int startRow = diagScoreLen - 1; startRow < vertScoreLen; startRow+=1) {
                    bool isWinner = true;
                    string? lineOwner = Spaces[startCol, startRow].Mark;
                    if(lineOwner != null) {
                        for(int d = 1; d < diagScoreLen; d+=1) {
                            if(lineOwner != Spaces[startCol + d, startRow - d].Mark) {
                                isWinner = false;
                                break;
                            }
                        }
                        if(isWinner) {
                            result.Add(new PlayerScore(new Player(lineOwner), 1));
                        }
                    }
                }
            }
            return new ScoreCard(result);
        }
    }
    #endregion
}
