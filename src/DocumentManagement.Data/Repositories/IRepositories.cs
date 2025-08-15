using DocumentManagement.Models;

namespace DocumentManagement.Data.Repositories;

/// <summary>
/// 通用仓储接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// 收文仓储接口
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByStatusAsync(DocumentStatus status);
    Task<IEnumerable<Document>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Document?> GetByDocumentNumberAsync(string documentNumber);
    Task<IEnumerable<Document>> SearchAsync(string searchTerm);
}

/// <summary>
/// 文档模板仓储接口
/// </summary>
public interface IDocumentTemplateRepository : IRepository<DocumentTemplate>
{
    Task<IEnumerable<DocumentTemplate>> GetEnabledTemplatesAsync();
    Task<DocumentTemplate?> GetWithControlsAsync(int templateId);
}

/// <summary>
/// 模板控件仓储接口
/// </summary>
public interface ITemplateControlRepository : IRepository<TemplateControl>
{
    Task<IEnumerable<TemplateControl>> GetByTemplateIdAsync(int templateId);
}