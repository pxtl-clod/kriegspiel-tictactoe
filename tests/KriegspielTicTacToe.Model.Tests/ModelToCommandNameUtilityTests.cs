namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using KriegspielTicTacToe.Model;

/// <summary>
/// Tests for ModelToCommandNameUtility class.
/// </summary>
public class ModelToCommandNameUtilityTests {
    
    #region Player Mark Mapping Tests
    
    [Fact]
    public void BuildPlayerToCommandNameMap_SingleTypeableMark() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("A") }
        );
        
        map.ContainsKey(new Player("A")).Should().BeTrue();
        map.ContainsKey(new Player("B")).Should().BeFalse();
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_TypeableMark_GetsMapping() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("@") }
        );
        
        map.ContainsKey(new Player("@")).Should().BeTrue();
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_MultiplePlayers() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { 
                new Player("A"),
                new Player("B"),
                new Player("@")
            }
        );
        
        map.ContainsKey(new Player("A")).Should().BeTrue();
        map.ContainsKey(new Player("B")).Should().BeTrue();
        map.ContainsKey(new Player("@")).Should().BeTrue();
    }

    #endregion

    #region Non-Typeable Mark Tests
    
    [Fact]
    public void BuildPlayerToCommandNameMap_EmptyString_ReturnEmpty() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("") }
        );
        
        map.Count.Should().Be(0);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_WhitespaceOnly_ReturnEmpty() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("   ") }
        );
        
        map.Count.Should().Be(0);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_DashChar_ReturnEmpty() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("-") }
        );
        
        map.Count.Should().Be(0);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_ColonChar_ReturnEmpty() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player(":") }
        );
        
        map.Count.Should().Be(0);
    }

    #endregion

    #region Edge Cases
    
    [Fact]
    public void BuildPlayerToCommandNameMap_EmptyArray() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(Array.Empty<Player>());
        
        map.Count.Should().Be(0);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_NullPlayers() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(null!);
        
        map.Count.Should().Be(0);
    }

    #endregion
}
