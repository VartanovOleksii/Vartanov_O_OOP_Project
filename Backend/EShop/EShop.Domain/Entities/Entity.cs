namespace EShop.Domain.Entities;

public abstract class Entity : IEquatable<Entity>
{
    public int Id { get; protected set; }

    public abstract bool IsValid();

    
    public bool Equals(Entity? other)
    {
        if (other == null)
            return false;
        
        if (Id == other.Id)
            return true;
        return false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}