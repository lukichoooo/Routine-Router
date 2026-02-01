
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Domain.SeedWork;

namespace Infrastructure.Persistence;


public interface ITrackedEntities
{
    void Add<TID>(AggregateRoot<TID> entity) where TID : AggregateRootId;
    void Remove<TID>(AggregateRoot<TID> entity) where TID : AggregateRootId;

    void Clear();

    ReadOnlyCollection<AggregateRoot<AggregateRootId>> GetAll();
}

public class TrackedEntities : ITrackedEntities
{
    private readonly ConcurrentDictionary<Type, List<object>> _trackedEntities = [];

    public void Add<TID>(AggregateRoot<TID> entity) where TID : AggregateRootId
    {
        var type = typeof(AggregateRoot<TID>);
        if (!_trackedEntities.ContainsKey(type))
            _trackedEntities[type] = [];

        _trackedEntities[type].Add(entity);
    }

    public void Remove<TID>(AggregateRoot<TID> entity) where TID : AggregateRootId
    {
        var type = typeof(AggregateRoot<TID>);
        if (_trackedEntities.TryGetValue(type, out var entities))
            entities.Remove(entity);
    }

    public void Clear() => _trackedEntities.Clear();
    public ReadOnlyCollection<AggregateRoot<AggregateRootId>> GetAll()
        => (ReadOnlyCollection<AggregateRoot<AggregateRootId>>)
        _trackedEntities.SelectMany(kv => kv.Value);
}

