namespace EShop.Domain.Interfaces;

public interface IJsonDataPort<T>
{
    string Export(IEnumerable<T> items);
    IEnumerable<T> Import(string json);
}
