using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.SeedWork
{
    public abstract class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);
        public void RemoveDomainEvent(IDomainEvent eventItem) => _domainEvents.Remove(eventItem);
        public void ClearDomainEvents() => _domainEvents.Clear();

        public bool IsTransient() => Id == default;

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            if (ReferenceEquals(this, obj)) return true;

            Entity other = (Entity)obj;
            if (IsTransient() || other.IsTransient()) return false;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return IsTransient()
                ? base.GetHashCode()
                : HashCode.Combine(Id);
        }

        public static bool operator ==(Entity left, Entity right) => Equals(left, right);
        public static bool operator !=(Entity left, Entity right) => !Equals(left, right);


        protected Entity()
        {
            Id = Guid.NewGuid(); // automatically assign a GUID
        }
    }
}
