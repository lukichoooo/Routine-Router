namespace Application.Interfaces.Data;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct = default);
}
