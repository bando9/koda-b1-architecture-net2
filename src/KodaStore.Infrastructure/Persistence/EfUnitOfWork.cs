using KodaStore.Application.Abstractions;
using KodaStore.Application.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace KodaStore.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext db, ILogger<EfUnitOfWork> logger) : IUnitOfWork
{
    // private readonly AppDbContext _db;
    // private readonly ILogger<EfUnitOfWork> _logger;

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var transaction = await db.Database.BeginTransactionAsync(ct);

        return new EfUnitOfWorkTransaction(transaction);
    }

    private sealed class EfUnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
    {
        public Task CommitAsync(CancellationToken ct) => transaction.CommitAsync(ct);

        public Task RollbackAsync(CancellationToken ct)
            => transaction.RollbackAsync(ct);

        public ValueTask DisposeAsync()
            => transaction.DisposeAsync();
    }

    public async Task<int> SaveChangeAsync(CancellationToken ct)
    {
        try
        {
            return await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException e)
        {
            logger.LogWarning(e, "Concurrency Conflict.");

            throw new DataConcurrencyException("Data telah berubah oleh request lain", e);
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new UniqueConstraintViolationException("Data dengan nilai unik sudah tersedia", e);
        }
    }
}