using DocumentManagement.Models;
using DocumentManagement.Data.Repositories;

namespace DocumentManagement.Core.Services;

/// <summary>
/// 文档管理服务接口
/// </summary>
public interface IDocumentService
{
    Task<Document> CreateDocumentAsync(Document document);
    Task<Document?> GetDocumentAsync(int id);
    Task<Document?> GetDocumentByNumberAsync(string documentNumber);
    Task<IEnumerable<Document>> GetAllDocumentsAsync();
    Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status);
    Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm);
    Task<Document> UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(int id);
    Task<Document> UpdateDocumentStatusAsync(int id, DocumentStatus status);
}

/// <summary>
/// 文档管理服务实现
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        // 验证收文编号是否唯一
        var existing = await _documentRepository.GetByDocumentNumberAsync(document.DocumentNumber);
        if (existing != null)
        {
            throw new InvalidOperationException($"收文编号 '{document.DocumentNumber}' 已存在");
        }

        document.CreatedAt = DateTime.Now;
        document.UpdatedAt = DateTime.Now;
        
        return await _documentRepository.AddAsync(document);
    }

    public async Task<Document?> GetDocumentAsync(int id)
    {
        return await _documentRepository.GetByIdAsync(id);
    }

    public async Task<Document?> GetDocumentByNumberAsync(string documentNumber)
    {
        return await _documentRepository.GetByDocumentNumberAsync(documentNumber);
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
    {
        return await _documentRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status)
    {
        return await _documentRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllDocumentsAsync();
        }
        
        return await _documentRepository.SearchAsync(searchTerm);
    }

    public async Task<Document> UpdateDocumentAsync(Document document)
    {
        var existing = await _documentRepository.GetByIdAsync(document.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"收文ID '{document.Id}' 不存在");
        }

        // 如果收文编号发生变化，需要验证唯一性
        if (existing.DocumentNumber != document.DocumentNumber)
        {
            var duplicateCheck = await _documentRepository.GetByDocumentNumberAsync(document.DocumentNumber);
            if (duplicateCheck != null && duplicateCheck.Id != document.Id)
            {
                throw new InvalidOperationException($"收文编号 '{document.DocumentNumber}' 已存在");
            }
        }

        document.UpdatedAt = DateTime.Now;
        return await _documentRepository.UpdateAsync(document);
    }

    public async Task DeleteDocumentAsync(int id)
    {
        var existing = await _documentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new InvalidOperationException($"收文ID '{id}' 不存在");
        }

        await _documentRepository.DeleteAsync(id);
    }

    public async Task<Document> UpdateDocumentStatusAsync(int id, DocumentStatus status)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            throw new InvalidOperationException($"收文ID '{id}' 不存在");
        }

        document.Status = status;
        document.UpdatedAt = DateTime.Now;
        
        return await _documentRepository.UpdateAsync(document);
    }
}