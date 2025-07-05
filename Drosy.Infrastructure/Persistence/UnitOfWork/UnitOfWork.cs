using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Drosy.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;
        private IDbContextTransaction? _currentTransaction;

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                var changes = await _context.SaveChangesAsync();
                return changes > 0
                    ? Result.Success()
                    : Result.Failure(Error.EFCore.NoChanges);
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Failure);
            }
        }
        public async Task<Result> StartTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                    return Result.Failure(Error.EFCore.FailedTransaction);

                _currentTransaction = await _context.Database.BeginTransactionAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Failure, ex);
            }
        }
        public async Task<Result> CommitAsync()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure(Error.Failure);

                var saveResult = await SaveChangesAsync();
                if (saveResult.IsFailure)
                {
                    return Result.Failure(Error.Failure);
                }
                await _currentTransaction.CommitAsync();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Failure, ex);
            }
        }
        public async Task<Result> RollbackAsync()
        {
            try
            {
                if (_currentTransaction == null)
                    return Result.Failure(Error.Failure);

                await _currentTransaction.RollbackAsync();
                CleanupTransaction();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.Failure, ex);
            }
        }
        private void CleanupTransaction()
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
        public void Dispose()
        {
            try
            {
                _context.Dispose();
                _currentTransaction?.Dispose();
            }
            catch (Exception ex)
            {
                // Handle dispose exception if needed
                throw new Exception($"An error occurred during disposal: {ex.Message}");
            }
        }
    }
}
