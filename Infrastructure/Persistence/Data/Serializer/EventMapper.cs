using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.SeedWork;
using Infrastructure.Persistence.Data.Exceptions;

namespace Infrastructure.Persistence.Data.Serializer;


public interface IJsonEventMapper
{
    string ToPayload(IDomainEvent domainEvent);
    IDomainEvent ToDomainEvent(Event dbEvent);
}


//     TODO: Requires Optimization


// <summary>
//     Serializes a domain event to a JSON Payload.
//     uses reflection
// </summary>
public class JsonEventMapper : IJsonEventMapper
{
    private readonly IReadOnlySet<Type> _eventTypes;
    private readonly IReadOnlySet<Type> _AggregateIdTypes;

    private readonly IReadOnlyDictionary<Type, List<PropertyInfo>> _typeToValidProps;

    private readonly IReadOnlyDictionary<string, Type> _typeNameToType;

    public JsonEventMapper()
    {
        _eventTypes = GetEventTypes();
        _AggregateIdTypes = GetAggregateIdTypes();

        _typeToValidProps = GetValidEventTypes();
        _typeNameToType = GetTypeNameToType();
    }


    public IDomainEvent ToDomainEvent(Event dbEvent)
    {
        if (!_typeNameToType.TryGetValue(dbEvent.EventType, out var domainEventType))
        {
            throw new EventSerializationException($"Unknown event type: {dbEvent.EventType}");
        }

        var node = JsonSerializer.Deserialize<JsonObject>(dbEvent.Payload)
            ?? throw new EventSerializationException($"Failed to deserialize event of type: {domainEventType.Name}");

        foreach (var (value, propName) in dbEvent.GetIgnoredOnPayloadFields())
        {
            if (value is Guid guid)
            {
                var idType = _typeNameToType[dbEvent.AggregateIdType];
                var id = Activator.CreateInstance(idType, guid);

                node.Add(propName, JsonSerializer.SerializeToNode(id));
            }
            else
            {
                node.Add(propName, JsonSerializer.SerializeToNode(value));
            }
        }

        var domainEvent = node.Deserialize(domainEventType)
            ?? throw new EventSerializationException($"Failed to deserialize event of type: {domainEventType.Name}");

        return (IDomainEvent)domainEvent;
    }

    public string ToPayload(IDomainEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();

        var node = new JsonObject();
        foreach (var prop in _typeToValidProps[domainEventType])
        {
            var value = prop.GetValue(domainEvent);

            var nodeVal = JsonSerializer.SerializeToNode(value);
            node.Add(prop.Name, nodeVal);
        }

        return node.ToString();
    }


    // Helper

    private ConcurrentDictionary<Type, List<PropertyInfo>> GetValidEventTypes()
    {
        ConcurrentDictionary<Type, List<PropertyInfo>> result = [];
        foreach (var eventType in _eventTypes)
        {
            result[eventType] = eventType.GetProperties()
                .Where(p => !Event.IgnoredOnPayloadFields.Contains(p.Name))
                .ToList();
        }
        return result;
    }

    private HashSet<Type> GetEventTypes()
    {
        var assemblyTypes = typeof(IDomainEvent).Assembly.GetTypes();

        var eventTypes = assemblyTypes.Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(IDomainEvent).IsAssignableFrom(t));

        return eventTypes.ToHashSet();
    }

    private HashSet<Type> GetAggregateIdTypes()
    {
        var assemblyTypes = typeof(IDomainEvent).Assembly.GetTypes();

        var aggregateIdTypes = assemblyTypes.Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(AggregateRootId).IsAssignableFrom(t));

        return aggregateIdTypes.ToHashSet();
    }


    private ConcurrentDictionary<string, Type> GetTypeNameToType()
    {
        ConcurrentDictionary<string, Type> result = [];
        foreach (var eventType in _eventTypes)
            result[eventType.Name] = eventType;
        foreach (var valueType in _AggregateIdTypes)
            result[valueType.Name] = valueType;

        return result;
    }

}

