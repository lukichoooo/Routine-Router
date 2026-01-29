namespace Domain.SeedWork;


public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(IDomainEvent eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;

        AggregateRoot other = (AggregateRoot)obj;
        return Id == other.Id;
    }

    public override int GetHashCode()
        => HashCode.Combine(Id);

    public static bool operator ==(AggregateRoot left, AggregateRoot right) => Equals(left, right);
    public static bool operator !=(AggregateRoot left, AggregateRoot right) => !Equals(left, right);


    protected AggregateRoot()
    {
        Id = Guid.NewGuid(); // automatically assign a GUID
    }
}

