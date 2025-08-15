using DocumentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentManagement.Services.Interfaces
{
    /// <summary>
    /// 文档管理服务接口
    /// </summary>
    public interface IDocumentService
    {
        Task<DocumentRecord> GetDocumentByIdAsync(int id);
        Task<IEnumerable<DocumentRecord>> GetAllDocumentsAsync();
        Task<IEnumerable<DocumentRecord>> SearchDocumentsAsync(string searchTerm);
        Task<IEnumerable<DocumentRecord>> FilterDocumentsAsync(Expression<Func<DocumentRecord, bool>> filter);
        Task<DocumentRecord> CreateDocumentAsync(DocumentRecord document);
        Task UpdateDocumentAsync(DocumentRecord document);
        Task DeleteDocumentAsync(int id);
        Task<bool> DocumentExistsAsync(int id);
        Task<IEnumerable<DocumentRecord>> GetDocumentsByStatusAsync(DocumentStatus status);
        Task<IEnumerable<DocumentRecord>> GetDocumentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    /// <summary>
    /// 数据联动服务接口
    /// </summary>
    public interface IDataLinkageService
    {
        Task<IEnumerable<string>> GetSenderUnitsAsync();
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<string>> GetHandlersAsync(string department = null);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<string> GetNextDocumentNumberAsync();
        Task<IEnumerable<string>> GetRelatedUnitsAsync(string senderUnit);
        Task AutoFillDocumentInfoAsync(DocumentRecord document, string senderUnit);
    }

    /// <summary>
    /// 表单生成服务接口
    /// </summary>
    public interface IFormGenerationService
    {
        Task<WpfFormDefinition> GenerateFormFromTemplateAsync(string templatePath);
        Task<object> CreateWpfFormAsync(WpfFormDefinition formDefinition);
        Task BindDataToFormAsync(object form, DocumentRecord data);
        Task<DocumentRecord> ExtractDataFromFormAsync(object form);
        Task ValidateFormAsync(object form);
    }
}