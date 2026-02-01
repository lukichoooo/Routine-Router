using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.SeedWork;
using Infrastructure.Persistence.Data.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Data;

public interface IEventSerializer
{
    string Serialize(IDomainEvent<AggregateRootId> domainEvent);
    IDomainEvent<AggregateRootId> Deserialize(string eventData, string eventType);
}


public class EventSerializer : IEventSerializer
{
    private readonly ConcurrentDictionary<string, Type> _eventTypeMap = new();
    private readonly ConcurrentDictionary<Type, string> _reverseEventTypeMap = new();
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<EventSerializer> _logger;

    public EventSerializer(ILogger<EventSerializer> logger)
    {
        _logger = logger;

        _serializerOptions = CreateSerializerOptions();
        RegisterEventTypes();
    }

    public string Serialize(IDomainEvent<AggregateRootId> @event)
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



    public IDomainEvent<AggregateRootId> Deserialize(string eventData, string eventType)
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

            return (IDomainEvent<AggregateRootId>)@event;
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
        var assembly = typeof(IDomainEvent<AggregateRootId>).Assembly;

        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                    && typeof(IDomainEvent<AggregateRootId>).IsAssignableFrom(t));

        foreach (var eventType in eventTypes)
        {
            var eventTypeString = eventType.Name;
            _eventTypeMap[eventTypeString] = eventType;
            _reverseEventTypeMap[eventType] = eventTypeString;
        }

        _logger.LogInformation($"Registered event types: {string.Join(", ", _eventTypeMap.Keys.ToArray())}");
    }


    private JsonSerializerOptions CreateSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // WriteIndented = false,
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }
}

