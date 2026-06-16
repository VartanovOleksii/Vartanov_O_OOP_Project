namespace EShop.Domain.Entities;

public abstract class Entity : IEquatable<Entity>
{
    public int Id { get; protected set; }

    public abstract bool IsValid();

    
    public bool Equals(Entity? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException();
    }

}