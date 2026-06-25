
using System.Reflection;
using KriegspielTicTacToe.Model.Template;

namespace KriegspielTicTacToe.Model;
public static partial class GameTemplates {
    public static IEnumerable<IGameTemplate> GetBuiltInGameTemplates()
        => typeof(GameTemplates)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(propInfo => propInfo.PropertyType.IsAssignableTo(typeof(IGameTemplate)))
            .Select(propInfo => (IGameTemplate)propInfo.GetValue(null)!);
}