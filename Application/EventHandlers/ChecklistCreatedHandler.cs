using Application.Seedwork;
using Domain.Entities.Schedules.Events;

namespace Application.EventHandlers;


public class ChecklistCreatedHandler : BaseDomainEventHandler<ChecklistCreated>
{
    protected override Task ExecuteAsync(ChecklistCreated evt, CancellationToken ct)
    {
        // TODO:
        Console.WriteLine(@$"Checklist created with Id={evt.AggregateId}, 
                Handled By ChecklistCreatedHandler");

        return Task.CompletedTask;
    }
}

