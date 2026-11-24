using KodaStore.Application.Repositories;

namespace KodaStore.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangeAsync(CancellationToken ct);
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct);
}


public interface IUnitOfWorkTransaction
{
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}

