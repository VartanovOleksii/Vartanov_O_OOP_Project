namespace EShop.Domain.Ports;

public class JsonDataPort<T>
{
    public string Export(IEnumerable<T> collection)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> Import(string json)
    {
        throw new NotImplementedException();
    }
}
