using DocumentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Interfaces
{
    /// <summary>
    /// 文档记录存储库接口
    /// </summary>
    public interface IDocumentRepository
    {
        Task<DocumentRecord> GetByIdAsync(int id);
        Task<IEnumerable<DocumentRecord>> GetAllAsync();
        Task<IEnumerable<DocumentRecord>> FindAsync(Expression<Func<DocumentRecord, bool>> predicate);
        Task<DocumentRecord> AddAsync(DocumentRecord entity);
        Task UpdateAsync(DocumentRecord entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IDocumentRepository Documents { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}