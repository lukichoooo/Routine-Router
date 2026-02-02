
using System.Collections.Concurrent;
using Domain.SeedWork;

namespace Infrastructure.Persistence;


public interface ITrackedEntities
{
    void Add(IAggregateRoot entity);
    void Remove(IAggregateRoot entity);

    void Clear();

    IEnumerable<IAggregateRoot> GetCollection();

    IReadOnlyDictionary<Type, HashSet<IAggregateRoot>> GetDictionary();
}

public class InMemoryTrackedEntities : ITrackedEntities
{
    private readonly ConcurrentDictionary<Type, HashSet<IAggregateRoot>> _trackedEntities = [];

    public void Add(IAggregateRoot entity)
    {
        var type = entity.GetType();
        if (!_trackedEntities.ContainsKey(type))
            _trackedEntities[type] = [];

        _trackedEntities[type].Add(entity);
    }

    public void Remove(IAggregateRoot entity)
    {
        var type = entity.GetType();
        if (_trackedEntities.TryGetValue(type, out var entities))
            entities.Remove(entity);
    }

    public void Clear() => _trackedEntities.Clear();

    public IEnumerable<IAggregateRoot> GetCollection()
        =>
        _trackedEntities.SelectMany(kv => kv.Value);

    public IReadOnlyDictionary<Type, HashSet<IAggregateRoot>> GetDictionary()
        => _trackedEntities.AsReadOnly();
}

