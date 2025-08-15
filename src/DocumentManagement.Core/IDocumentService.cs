using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentManagement.Models;

namespace DocumentManagement.Core
{
    /// <summary>
    /// 文档服务接口
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// 获取所有文档
        /// </summary>
        /// <returns>文档列表</returns>
        Task<IEnumerable<Document>> GetAllDocumentsAsync();

        /// <summary>
        /// 根据ID获取文档
        /// </summary>
        /// <param name="id">文档ID</param>
        /// <returns>文档对象</returns>
        Task<Document> GetDocumentByIdAsync(int id);

        /// <summary>
        /// 创建新文档
        /// </summary>
        /// <param name="document">文档对象</param>
        /// <returns>创建的文档对象</returns>
        Task<Document> CreateDocumentAsync(Document document);

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="document">文档对象</param>
        /// <returns>更新的文档对象</returns>
        Task<Document> UpdateDocumentAsync(Document document);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="id">文档ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteDocumentAsync(int id);

        /// <summary>
        /// 根据条件搜索文档
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="departmentId">部门ID</param>
        /// <param name="status">状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>符合条件的文档列表</returns>
        Task<IEnumerable<Document>> SearchDocumentsAsync(
            string keyword = null,
            int? departmentId = null,
            string status = null,
            DateTime? startDate = null,
            DateTime? endDate = null);

        /// <summary>
        /// 获取待办文档列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>待办文档列表</returns>
        Task<IEnumerable<Document>> GetPendingDocumentsAsync(int userId);

        /// <summary>
        /// 更新文档状态
        /// </summary>
        /// <param name="documentId">文档ID</param>
        /// <param name="status">新状态</param>
        /// <param name="userId">操作用户ID</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateDocumentStatusAsync(int documentId, string status, int userId);

        /// <summary>
        /// 添加文档附件
        /// </summary>
        /// <param name="documentId">文档ID</param>
        /// <param name="fileName">文件名</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否添加成功</returns>
        Task<bool> AddDocumentAttachmentAsync(int documentId, string fileName, string filePath);

        /// <summary>
        /// 获取文档统计信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>统计信息</returns>
        Task<object> GetDocumentStatisticsAsync(int? userId = null);

        /// <summary>
        /// 分页获取文档
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>分页文档结果</returns>
        Task<object> GetDocumentsPagedAsync(int pageIndex, int pageSize, string sortField = "CreatedTime", string sortDirection = "desc");
    }
}