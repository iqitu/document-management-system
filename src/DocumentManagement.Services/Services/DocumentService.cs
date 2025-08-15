using DocumentManagement.Core.Interfaces;
using DocumentManagement.Models;
using DocumentManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentManagement.Services.Services
{
    /// <summary>
    /// 文档管理服务实现
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<DocumentRecord> GetDocumentByIdAsync(int id)
        {
            return await _unitOfWork.Documents.GetByIdAsync(id);
        }

        public async Task<IEnumerable<DocumentRecord>> GetAllDocumentsAsync()
        {
            return await _unitOfWork.Documents.GetAllAsync();
        }

        public async Task<IEnumerable<DocumentRecord>> SearchDocumentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllDocumentsAsync();

            return await _unitOfWork.Documents.FindAsync(d =>
                d.Title.Contains(searchTerm) ||
                d.DocumentNumber.Contains(searchTerm) ||
                d.SenderUnit.Contains(searchTerm) ||
                d.Handler.Contains(searchTerm));
        }

        public async Task<IEnumerable<DocumentRecord>> FilterDocumentsAsync(Expression<Func<DocumentRecord, bool>> filter)
        {
            return await _unitOfWork.Documents.FindAsync(filter);
        }

        public async Task<DocumentRecord> CreateDocumentAsync(DocumentRecord document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.CreatedDate = DateTime.Now;
            return await _unitOfWork.Documents.AddAsync(document);
        }

        public async Task UpdateDocumentAsync(DocumentRecord document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.ModifiedDate = DateTime.Now;
            await _unitOfWork.Documents.UpdateAsync(document);
        }

        public async Task DeleteDocumentAsync(int id)
        {
            await _unitOfWork.Documents.DeleteAsync(id);
        }

        public async Task<bool> DocumentExistsAsync(int id)
        {
            return await _unitOfWork.Documents.ExistsAsync(id);
        }

        public async Task<IEnumerable<DocumentRecord>> GetDocumentsByStatusAsync(DocumentStatus status)
        {
            return await _unitOfWork.Documents.FindAsync(d => d.Status == status);
        }

        public async Task<IEnumerable<DocumentRecord>> GetDocumentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Documents.FindAsync(d =>
                d.ReceiveDate >= startDate && d.ReceiveDate <= endDate);
        }
    }

    /// <summary>
    /// 数据联动服务实现
    /// </summary>
    public class DataLinkageService : IDataLinkageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataLinkageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<string>> GetSenderUnitsAsync()
        {
            var documents = await _unitOfWork.Documents.GetAllAsync();
            return documents.Where(d => !string.IsNullOrEmpty(d.SenderUnit))
                          .Select(d => d.SenderUnit)
                          .Distinct()
                          .OrderBy(s => s);
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            var documents = await _unitOfWork.Documents.GetAllAsync();
            return documents.Where(d => !string.IsNullOrEmpty(d.Department))
                          .Select(d => d.Department)
                          .Distinct()
                          .OrderBy(s => s);
        }

        public async Task<IEnumerable<string>> GetHandlersAsync(string department = null)
        {
            var documents = await _unitOfWork.Documents.GetAllAsync();
            var query = documents.Where(d => !string.IsNullOrEmpty(d.Handler));

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(d => d.Department == department);
            }

            return query.Select(d => d.Handler)
                        .Distinct()
                        .OrderBy(s => s);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var documents = await _unitOfWork.Documents.GetAllAsync();
            return documents.Where(d => !string.IsNullOrEmpty(d.Category))
                          .Select(d => d.Category)
                          .Distinct()
                          .OrderBy(s => s);
        }

        public async Task<string> GetNextDocumentNumberAsync()
        {
            var today = DateTime.Today;
            var yearMonth = today.ToString("yyyyMM");
            
            var documents = await _unitOfWork.Documents.FindAsync(d =>
                d.DocumentNumber.StartsWith(yearMonth));

            var maxNumber = documents.Where(d => d.DocumentNumber.Length >= 10)
                                   .Select(d => d.DocumentNumber.Substring(6, 4))
                                   .Where(s => int.TryParse(s, out _))
                                   .Select(int.Parse)
                                   .DefaultIfEmpty(0)
                                   .Max();

            return $"{yearMonth}{(maxNumber + 1):D4}";
        }

        public async Task<IEnumerable<string>> GetRelatedUnitsAsync(string senderUnit)
        {
            if (string.IsNullOrEmpty(senderUnit))
                return new List<string>();

            var documents = await _unitOfWork.Documents.FindAsync(d =>
                d.SenderUnit == senderUnit);

            return documents.Where(d => !string.IsNullOrEmpty(d.Department))
                          .Select(d => d.Department)
                          .Distinct()
                          .OrderBy(s => s);
        }

        public async Task AutoFillDocumentInfoAsync(DocumentRecord document, string senderUnit)
        {
            if (document == null || string.IsNullOrEmpty(senderUnit))
                return;

            var recentDocuments = await _unitOfWork.Documents.FindAsync(d =>
                d.SenderUnit == senderUnit);

            var latestDocument = recentDocuments.OrderByDescending(d => d.ReceiveDate)
                                              .FirstOrDefault();

            if (latestDocument != null)
            {
                document.Category = latestDocument.Category;
                document.Department = latestDocument.Department;
            }
        }
    }
}