using System.Text.Json;

namespace FrontendPage.Data;

public static class JsonHelper
{
    private static readonly JsonWriterOptions _options = new JsonWriterOptions
    {
        Indented = true,
    };

    public static string PrettyPrint(string jsonString)
    {
        using JsonDocument document = JsonDocument.Parse(jsonString);
        using var stream = new System.IO.MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, _options))
        {
            // PrettyPriunt Json Document as is
            document.WriteTo(writer);
        }
        var prettyJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        return prettyJson;
    }
}
