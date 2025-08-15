using DocumentManagement.Core.Interfaces;
using DocumentManagement.Data.Context;
using DocumentManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentManagement.Data.Repositories
{
    /// <summary>
    /// 文档记录存储库实现
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentDbContext _context;

        public DocumentRepository(DocumentDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DocumentRecord> GetByIdAsync(int id)
        {
            return await _context.DocumentRecords
                .Include(d => d.Attachments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DocumentRecord>> GetAllAsync()
        {
            return await _context.DocumentRecords
                .Include(d => d.Attachments)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentRecord>> FindAsync(Expression<Func<DocumentRecord, bool>> predicate)
        {
            return await _context.DocumentRecords
                .Include(d => d.Attachments)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<DocumentRecord> AddAsync(DocumentRecord entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.CreatedDate = DateTime.Now;
            _context.DocumentRecords.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(DocumentRecord entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.ModifiedDate = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.DocumentRecords.FindAsync(id);
            if (entity != null)
            {
                _context.DocumentRecords.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.DocumentRecords.AnyAsync(d => d.Id == id);
        }
    }

    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DocumentDbContext _context;
        private IDocumentRepository _documents;
        private bool _disposed = false;

        public UnitOfWork(DocumentDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDocumentRepository Documents
        {
            get { return _documents ??= new DocumentRepository(_context); }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await Task.Run(() => _context.Database.BeginTransaction());
        }

        public async Task CommitTransactionAsync()
        {
            await Task.Run(() => _context.Database.CurrentTransaction?.Commit());
        }

        public async Task RollbackTransactionAsync()
        {
            await Task.Run(() => _context.Database.CurrentTransaction?.Rollback());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}