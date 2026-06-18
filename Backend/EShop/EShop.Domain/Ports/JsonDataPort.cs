using System.Text.Json;
using EShop.Domain.Interfaces;

namespace EShop.Domain.Ports;

public class JsonDataPort<T> : IJsonDataPort<T>
{
    public string Export(IEnumerable<T> collection)
    {
        if (collection == null)
            return "[]";

        return JsonSerializer.Serialize(collection);
    }

    public IEnumerable<T> Import(string json)
    {
        if (string.IsNullOrEmpty(json))
            return Enumerable.Empty<T>();

        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
}
