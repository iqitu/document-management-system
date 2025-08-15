using Microsoft.EntityFrameworkCore;
using DocumentManagement.Models;

namespace DocumentManagement.Data.Repositories;

/// <summary>
/// 通用仓储实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DocumentManagementContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DocumentManagementContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}

/// <summary>
/// 收文仓储实现
/// </summary>
public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(DocumentManagementContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Document>> GetByStatusAsync(DocumentStatus status)
    {
        return await _dbSet
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.ReceivedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(d => d.ReceivedDate >= startDate && d.ReceivedDate <= endDate)
            .OrderByDescending(d => d.ReceivedDate)
            .ToListAsync();
    }

    public async Task<Document?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DocumentNumber == documentNumber);
    }

    public async Task<IEnumerable<Document>> SearchAsync(string searchTerm)
    {
        return await _dbSet
            .Where(d => d.Title.Contains(searchTerm) || 
                       d.Content.Contains(searchTerm) ||
                       d.Sender.Contains(searchTerm))
            .OrderByDescending(d => d.ReceivedDate)
            .ToListAsync();
    }
}

/// <summary>
/// 文档模板仓储实现
/// </summary>
public class DocumentTemplateRepository : Repository<DocumentTemplate>, IDocumentTemplateRepository
{
    public DocumentTemplateRepository(DocumentManagementContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DocumentTemplate>> GetEnabledTemplatesAsync()
    {
        return await _dbSet
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<DocumentTemplate?> GetWithControlsAsync(int templateId)
    {
        return await _context.DocumentTemplates
            .Include(t => t.TemplateControls)
            .FirstOrDefaultAsync(t => t.Id == templateId);
    }
}

/// <summary>
/// 模板控件仓储实现
/// </summary>
public class TemplateControlRepository : Repository<TemplateControl>, ITemplateControlRepository
{
    public TemplateControlRepository(DocumentManagementContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TemplateControl>> GetByTemplateIdAsync(int templateId)
    {
        return await _dbSet
            .Where(c => c.TemplateId == templateId)
            .OrderBy(c => c.Y)
            .ThenBy(c => c.X)
            .ToListAsync();
    }
}