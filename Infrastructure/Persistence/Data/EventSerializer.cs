using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.SeedWork;
using Infrastructure.Persistence.Data.Exceptions;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<EventSerializer> _logger;

    public EventSerializer(ILogger<EventSerializer> logger)
    {
        _logger = logger;

        _serializerOptions = CreateSerializerOptions();
        RegisterEventTypes();
    }

    public string Serialize(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new EventSerializerArgumentNullException(nameof(domainEvent));

        try
        {
            var eventType = domainEvent.GetType();

            if (!_reverseEventTypeMap.TryGetValue(eventType, out var eventTypeString))
            {
                throw new EventSerializationException(
                        $"Event type '{eventType.FullName}' is not registered.");
            }

            return JsonSerializer.Serialize(domainEvent, eventType, _serializerOptions);
        }
        catch (JsonException ex)
        {
            throw new EventSerializationException(
                    $"Failed to serialize event of type '{domainEvent.GetType().Name}'", ex);
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

            return (IDomainEvent)@event;
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
        IEnumerable<Assembly> assemblies =
        [
            typeof(Domain.Entities.Schedules.Events.ChecklistCreated).Assembly,
            typeof(Domain.Entities.Users.Events.UserCreated).Assembly
        ];

        foreach (var assembly in assemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IDomainEvent).IsAssignableFrom(t));

            foreach (var eventType in eventTypes)
            {
                var eventTypeString = eventType.FullName ?? eventType.Name;
                _eventTypeMap.TryAdd(eventTypeString, eventType);
                _reverseEventTypeMap.TryAdd(eventType, eventTypeString);
            }
        }

        _logger.LogInformation($"Registered event types: {string.Join(", ", _eventTypeMap.Keys)}");
    }


    private JsonSerializerOptions CreateSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public IReadOnlyDictionary<string, Type> GetRegisteredEventTypes()
    {
        return _eventTypeMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}

