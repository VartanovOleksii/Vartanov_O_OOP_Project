using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace EShop.Domain.Ports;

public class JsonDataPort<T>
{
    public string Export(IEnumerable<T> collection)
    {
        if (collection == null)
            return "[]";

        string json = System.Text.Json.JsonSerializer.Serialize(collection);

        return json;
    }

    public IEnumerable<T> Import(string json)
    {
        if (string.IsNullOrEmpty(json))
            return Enumerable.Empty<T>();

        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
}
