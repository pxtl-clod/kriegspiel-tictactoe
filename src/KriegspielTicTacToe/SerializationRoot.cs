namespace KriegspielTicTacToe;

using Newtonsoft.Json;

/// <summary>
/// Forces Newtonsoft to serialize the type metadata about the root object.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SerializationRoot<T> {
    public SerializationRoot (){}
    public SerializationRoot (T content) {
        Content = content;
    }
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    public T? Content { get; set; }
}
