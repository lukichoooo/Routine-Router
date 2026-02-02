using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.SeedWork;
using Infrastructure.Persistence.Data.Exceptions;

namespace Infrastructure.Persistence.Data;

public interface IEventSerializer
{
    string Serialize(IDomainEvent domainEvent);
    IDomainEvent Deserialize(string eventData, string eventType);
}


public class EventSerializer : IEventSerializer
{
    private readonly ConcurrentDictionary<string, Type> _eventTypeMap = new();
    private readonly ConcurrentDictionary<Type, string> _reverseEventTypeMap = new();
    private readonly JsonSerializerOptions _serializerOptions;

    public EventSerializer()
    {
        _serializerOptions = CreateSerializerOptions();
        RegisterEventTypes();
    }

    public string Serialize(IDomainEvent @event)
    {
        if (@event == null)
            throw new EventSerializerArgumentNullException(nameof(@event));

        try
        {
            var eventType = @event.GetType();

            if (!_reverseEventTypeMap.TryGetValue(eventType, out var eventTypeString))
            {
                throw new EventSerializationException(
                        $"Event type '{eventType.Name}' is not registered.");
            }

            return JsonSerializer.Serialize(@event, eventType, _serializerOptions);
        }
        catch (JsonException ex)
        {
            throw new EventSerializationException(
                    $"Failed to serialize event of type '{@event.GetType().Name}'", ex);
        }
    }



    public IDomainEvent Deserialize(string eventData, string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventData))
            throw new EventSerializerArgumentNullException(nameof(eventData));
        if (string.IsNullOrWhiteSpace(eventType))
            throw new EventSerializerArgumentNullException(nameof(eventType));

        try
        {
            if (!_eventTypeMap.TryGetValue(eventType, out var concreteEventType))
            {
                throw new EventSerializationException($"Unknown event type: '{eventType}'.");
            }

            var @event = JsonSerializer.Deserialize(eventData, concreteEventType, _serializerOptions)
                ?? throw new EventSerializationException(
                    $"Deserialization returned null for event type: {eventType}");

            return (BaseDomainEvent<AggregateRootId>)@event;
        }
        catch (Exception ex)
        {
            throw new EventSerializationException(eventType, eventData, ex);
        }
    }


    // Helpers



    private void RegisterEventTypes()
    {
        // Scan assemblies
        var assembly = typeof(BaseDomainEvent<AggregateRootId>).Assembly;

        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                    && typeof(BaseDomainEvent<AggregateRootId>).IsAssignableFrom(t));

        foreach (var eventType in eventTypes)
        {
            var eventTypeString = eventType.Name;
            _eventTypeMap[eventTypeString] = eventType;
            _reverseEventTypeMap[eventType] = eventTypeString;
        }
    }


    private JsonSerializerOptions CreateSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true, // pretty
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}

