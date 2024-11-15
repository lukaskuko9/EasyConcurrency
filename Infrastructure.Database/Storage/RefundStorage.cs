using Core.Refund;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TimeLockExtensions = Infrastructure.Database.Extensions.TimeLockExtensions;

namespace Infrastructure.Database.Storage;

public class RefundStorage(DatabaseContext dbContext) : IRefundStorage
{
    public async Task InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token)
    {
        var exists = await dbContext.Refunds.AnyAsync(x => x.AccountKey == refundEntity.AccountKey, cancellationToken: token);
        if (exists)
            return;
        try
        {
            dbContext.Refunds.Add(Map(refundEntity));
            await dbContext.SaveChangesAsync(token);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException )
        {
            if(sqlException.Number == 2601)
                return;

            throw;
        }
    }

    public async Task SetProcessedAsync(RefundEntity entityToUpdate, CancellationToken token = default)
    {
        await dbContext.Refunds
            .Where(dbEntity => dbEntity.AccountKey == entityToUpdate.AccountKey)
            .ExecuteUpdateAsync(propCalls =>
                    propCalls.SetProperty(refundEntity => refundEntity.IsProcessed, true),
                cancellationToken: token
            );
    }

    public async Task<RefundEntity?> GetAndLockSingleRefundToProcess(CancellationToken token)
    {
        var dbEntity = await dbContext.Refunds
            //.Where(TimeLockExtensions.IsNotLocked<Entities.RefundEntity>())
            .WhereIsNotLocked()
            .Where(refundEntity => refundEntity.IsProcessed == false)
            .FirstOrDefaultAsync(token);

        if (dbEntity == null)
            return null;

        var locked = dbEntity.SetLock(5);
        
        if (locked == false) 
            return null;
        
        await dbContext.SaveChangesAsync(token);
        return Map(dbEntity);

    }

    private static RefundEntity? Map(Entities.RefundEntity? core) =>
        core == null ? null : new RefundEntity(core.AccountKey, core.Amount)
            {
                Id = core.Id
            };
    
    private static Entities.RefundEntity Map(RefundEntity core) => 
        new(core.Id, core.AccountKey, core.Amount);
}