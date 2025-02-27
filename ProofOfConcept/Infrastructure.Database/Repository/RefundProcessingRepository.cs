﻿using Core.Refund;
using EasyConcurrency.EntityFramework.ConcurrentRepository;
using EasyConcurrency.EntityFramework.LockableEntity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repository
{
    public class RefundProcessingRepository(DatabaseContext dbContext) : ConcurrentRepository<DatabaseContext>(dbContext), IRefundProcessingRepository
    {
        private readonly DatabaseContext _dbContext = dbContext;

        public async Task<bool> InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token)
        {
            var exists = await _dbContext.Refunds.AnyAsync(x => x.AccountKey == refundEntity.AccountKey, cancellationToken: token);
            if (exists)
                return false;

            var mapped = Map(refundEntity);
            return await InsertAndSaveAsync(mapped, token);
        }

        public async Task DeleteAsync(RefundEntity entityToUpdate, CancellationToken token = default)
        {
            await _dbContext.Refunds
                .Where(dbEntity => dbEntity.AccountKey == entityToUpdate.AccountKey)
                .ExecuteDeleteAsync(cancellationToken: token);
        }

        public async Task<RefundEntity?> GetAndLockFirstOrDefaultAsync(CancellationToken token)
        {
            var refundEntity = await _dbContext.Refunds
                .WhereIsNotLocked()
                .FirstOrDefaultAsync(token);

            var dbEntity = await LockAndSaveAsync(refundEntity, TimeSpan.FromMinutes(5), cancellationToken: token);
            return Map(dbEntity);

        }

        private static RefundEntity? Map(Entities.RefundEntity? core) =>
            core == null ? null : new RefundEntity(core.AccountKey, core.Amount)
            {
                Id = core.Id,
                AccountKey = core.AccountKey,
                IsProcessed = core.IsProcessed,
                Amount = core.Amount
            };

        private static Entities.RefundEntity Map(RefundEntity core) =>
            new()
            {
                Id = core.Id,
                AccountKey = core.AccountKey,
                IsProcessed = core.IsProcessed,
                Amount = core.Amount
            };
    }
}