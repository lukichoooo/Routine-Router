using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.SeedWork;
using Infrastructure.Persistence.Data.Exceptions;

namespace Infrastructure.Persistence.Data
{
    public class EventSerializer
    {
        private readonly ConcurrentDictionary<string, Type> _eventTypeMap = new();
        private readonly ConcurrentDictionary<Type, string> _reverseEventTypeMap = new();
        private readonly JsonSerializerOptions _serializerOptions;

        public EventSerializer()
        {
            _serializerOptions = CreateSerializerOptions();
            RegisterEventTypes();
        }

        public string Serialize<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent<object>
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

        public IDomainEvent<object> Deserialize(string eventData, string eventType, Type aggregateIdType)
        {
            if (string.IsNullOrWhiteSpace(eventData))
                throw new EventSerializerArgumentNullException(nameof(eventData));
            if (string.IsNullOrWhiteSpace(eventType))
                throw new EventSerializerArgumentNullException(nameof(eventType));
            if (aggregateIdType == null)
                throw new EventSerializerArgumentNullException(nameof(aggregateIdType));

            try
            {
                if (!_eventTypeMap.TryGetValue(eventType, out var concreteEventType))
                {
                    throw new EventSerializationException($"Unknown event type: '{eventType}'.");
                }

                var expectedAggregateIdType = GetAggregateIdType(concreteEventType);
                if (expectedAggregateIdType != aggregateIdType)
                {
                    throw new EventSerializationException(
                        $"Aggregate ID type mismatch. Expected: {expectedAggregateIdType.Name}, " +
                        $"Provided: {aggregateIdType.Name} for event type: {eventType}");
                }

                var @event = JsonSerializer.Deserialize(eventData, concreteEventType, _serializerOptions);
                if (@event == null)
                {
                    throw new EventSerializationException(
                        $"Deserialization returned null for event type: {eventType}");
                }

                return (IDomainEvent<object>)@event;
            }
            catch (Exception ex)
            {
                throw new EventSerializationException(eventType, eventData, ex);
            }
        }

        public IDomainEvent<TAggregateId> Deserialize<TAggregateId>(string eventData, string eventType)
        {
            var result = Deserialize(eventData, eventType, typeof(TAggregateId));
            return (IDomainEvent<TAggregateId>)result;
        }

        private void RegisterEventTypes()
        {
            IEnumerable<Assembly> assemblies =
            [
                typeof(Domain.Entities.Schedules.Events.ChecklistCreated).Assembly,
                typeof(Domain.Entities.Users.Events.UserCreated).Assembly
            ];

            foreach (var assembly in assemblies)
            {
                var eventTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEvent<>)))
                    .ToList();

                foreach (var eventType in eventTypes)
                {
                    var eventTypeString = eventType.FullName ?? eventType.Name;
                    _eventTypeMap.TryAdd(eventTypeString, eventType);
                    _reverseEventTypeMap.TryAdd(eventType, eventTypeString);
                }
            }
        }

        private Type GetAggregateIdType(Type eventType)
        {
            var domainEventInterface = eventType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDomainEvent<>));

            return domainEventInterface?.GetGenericArguments()[0] ?? typeof(object);
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
}
