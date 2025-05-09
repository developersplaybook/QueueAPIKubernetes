using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.DAL;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Repositories;

public class QueueRepository : IClientQueueRepository, IServerQueueRepository
{
    private readonly IConfiguration _configuration;

    public QueueRepository(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<QueueEntity?> GetMessageFromClientQueueAsync()
    {
        await using var context = GetContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var item = await context.ClientQueue
                .Where(q => q.QueueStatus == QueueStatus.New)
                .OrderBy(q => q.Created)
                .FirstOrDefaultAsync();

            if (item is null)
            {
                await transaction.RollbackAsync();
                return null;
            }

            item.QueueStatus = QueueStatus.Processed;
            item.StatusDate = DateTime.UtcNow;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return item;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<QueueEntity?> GetMessageFromServerByCorrelationIdAsync(Guid correlationId)
    {
        await using var context = GetContext();

        var item = await context.ServerQueue
            .FirstOrDefaultAsync(q => q.CorrelationId == correlationId);

        if (item is null) return null;

        item.QueueStatus = QueueStatus.Processed;
        item.StatusDate = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return item;
    }

    public async Task<int> AddClientQueueItemAsync(QueueEntity entity)
    {
        await using var context = GetContext();
        context.ClientQueue.Add(ConvertTo<ClientQueueEntity>(entity));
        return await context.SaveChangesAsync();
    }

    public async Task<int> AddServerQueueItemAsync(QueueEntity entity)
    {
        await using var context = GetContext();
        context.ServerQueue.Add(ConvertTo<ServerQueueEntity>(entity));
        return await context.SaveChangesAsync();
    }

    private static T ConvertTo<T>(QueueEntity entity) where T : QueueEntity, new()
    {
        return new T
        {
            Id = entity.Id,
            CorrelationId = entity.CorrelationId,
            Created = entity.Created,
            StatusDate = entity.StatusDate,
            QueueStatus = entity.QueueStatus,
            TypeName = entity.TypeName,
            Content = entity.Content
        };
    }

    private QueueDbContext GetContext()
    {
        return new QueueDbContext(_configuration.GetConnectionString("QueueDbConnection"));
    }
}
