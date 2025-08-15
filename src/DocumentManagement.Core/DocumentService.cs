using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentManagement.Models;
using DocumentManagement.Common;

namespace DocumentManagement.Core
{
    /// <summary>
    /// 文档服务实现
    /// </summary>
    public class DocumentService : IDocumentService
    {
        // 注意：这是一个基础实现，实际项目中应该注入数据访问层
        private readonly List<Document> _documents;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DocumentService()
        {
            _documents = new List<Document>();
            InitializeSampleData();
        }

        /// <summary>
        /// 初始化示例数据
        /// </summary>
        private void InitializeSampleData()
        {
            var sampleDocument = new Document
            {
                Id = 1,
                Title = "关于召开年度工作会议的通知",
                DocumentNumber = Helpers.GenerateDocumentNumber("DOC"),
                SenderUnit = "办公室",
                ReceivedDate = DateTime.Now.AddDays(-3),
                UrgencyLevel = Constants.UrgencyLevel.Normal,
                SecurityLevel = Constants.SecurityLevel.Internal,
                Status = Constants.DocumentStatus.Pending,
                Content = "根据年度工作安排，定于下月15日召开年度工作会议，请各部门做好准备。",
                CreatedTime = DateTime.Now.AddDays(-3),
                UpdatedTime = DateTime.Now.AddDays(-3)
            };
            _documents.Add(sampleDocument);
        }

        /// <summary>
        /// 获取所有文档
        /// </summary>
        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await Task.FromResult(_documents.AsEnumerable());
        }

        /// <summary>
        /// 根据ID获取文档
        /// </summary>
        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await Task.FromResult(_documents.FirstOrDefault(d => d.Id == id));
        }

        /// <summary>
        /// 创建新文档
        /// </summary>
        public async Task<Document> CreateDocumentAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.Id = _documents.Count > 0 ? _documents.Max(d => d.Id) + 1 : 1;
            document.CreatedTime = DateTime.Now;
            document.UpdatedTime = DateTime.Now;
            
            if (string.IsNullOrEmpty(document.DocumentNumber))
            {
                document.DocumentNumber = Helpers.GenerateDocumentNumber("DOC");
            }

            if (string.IsNullOrEmpty(document.Status))
            {
                document.Status = Constants.DocumentStatus.Draft;
            }

            _documents.Add(document);
            return await Task.FromResult(document);
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        public async Task<Document> UpdateDocumentAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var existingDocument = _documents.FirstOrDefault(d => d.Id == document.Id);
            if (existingDocument == null)
                throw new InvalidOperationException($"Document with ID {document.Id} not found");

            // 更新属性
            existingDocument.Title = document.Title;
            existingDocument.SenderUnit = document.SenderUnit;
            existingDocument.UrgencyLevel = document.UrgencyLevel;
            existingDocument.SecurityLevel = document.SecurityLevel;
            existingDocument.Content = document.Content;
            existingDocument.Status = document.Status;
            existingDocument.UpdatedTime = DateTime.Now;

            return await Task.FromResult(existingDocument);
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            if (document != null)
            {
                _documents.Remove(document);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        /// <summary>
        /// 根据条件搜索文档
        /// </summary>
        public async Task<IEnumerable<Document>> SearchDocumentsAsync(
            string keyword = null,
            int? departmentId = null,
            string status = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _documents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d => d.Title.Contains(keyword) || 
                                       d.Content.Contains(keyword) ||
                                       d.SenderUnit.Contains(keyword));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(d => d.PrimaryDepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(d => d.ReceivedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.ReceivedDate <= endDate.Value);
            }

            return await Task.FromResult(query.ToList());
        }

        /// <summary>
        /// 获取待办文档列表
        /// </summary>
        public async Task<IEnumerable<Document>> GetPendingDocumentsAsync(int userId)
        {
            var pendingDocuments = _documents.Where(d => 
                d.Status == Constants.DocumentStatus.Pending &&
                (d.ResponsibleUserId == userId || d.CreatedUserId == userId))
                .ToList();

            return await Task.FromResult(pendingDocuments);
        }

        /// <summary>
        /// 更新文档状态
        /// </summary>
        public async Task<bool> UpdateDocumentStatusAsync(int documentId, string status, int userId)
        {
            var document = _documents.FirstOrDefault(d => d.Id == documentId);
            if (document != null)
            {
                document.Status = status;
                document.UpdatedTime = DateTime.Now;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        /// <summary>
        /// 添加文档附件
        /// </summary>
        public async Task<bool> AddDocumentAttachmentAsync(int documentId, string fileName, string filePath)
        {
            var document = _documents.FirstOrDefault(d => d.Id == documentId);
            if (document != null)
            {
                document.AttachmentPath = filePath;
                document.UpdatedTime = DateTime.Now;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        /// <summary>
        /// 获取文档统计信息
        /// </summary>
        public async Task<object> GetDocumentStatisticsAsync(int? userId = null)
        {
            var query = _documents.AsQueryable();
            
            if (userId.HasValue)
            {
                query = query.Where(d => d.ResponsibleUserId == userId.Value || d.CreatedUserId == userId.Value);
            }

            var statistics = new
            {
                TotalCount = query.Count(),
                PendingCount = query.Count(d => d.Status == Constants.DocumentStatus.Pending),
                InProgressCount = query.Count(d => d.Status == Constants.DocumentStatus.InProgress),
                CompletedCount = query.Count(d => d.Status == Constants.DocumentStatus.Completed),
                TodayReceivedCount = query.Count(d => d.ReceivedDate.Date == DateTime.Today),
                UrgentCount = query.Count(d => d.UrgencyLevel == Constants.UrgencyLevel.Urgent || 
                                              d.UrgencyLevel == Constants.UrgencyLevel.VeryUrgent)
            };

            return await Task.FromResult(statistics);
        }

        /// <summary>
        /// 分页获取文档
        /// </summary>
        public async Task<object> GetDocumentsPagedAsync(int pageIndex, int pageSize, string sortField = "CreatedTime", string sortDirection = "desc")
        {
            var query = _documents.AsQueryable();

            // 简单的排序实现
            if (sortField == "CreatedTime")
            {
                query = sortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(d => d.CreatedTime)
                    : query.OrderBy(d => d.CreatedTime);
            }
            else if (sortField == "Title")
            {
                query = sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.Title)
                    : query.OrderBy(d => d.Title);
            }

            var totalCount = query.Count();
            var documents = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var result = new
            {
                Data = documents,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return await Task.FromResult(result);
        }
    }
}