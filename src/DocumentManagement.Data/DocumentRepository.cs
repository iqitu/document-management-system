using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocumentManagement.Models;

namespace DocumentManagement.Data
{
    /// <summary>
    /// 文档仓储实现
    /// </summary>
    public class DocumentRepository : IRepository<Document>
    {
        private readonly DocumentContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据上下文</param>
        public DocumentRepository(DocumentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 获取所有文档
        /// </summary>
        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取文档
        /// </summary>
        public async Task<Document> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// 根据条件查找文档
        /// </summary>
        public async Task<IEnumerable<Document>> FindAsync(Expression<Func<Document, bool>> predicate)
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// 获取单个文档
        /// </summary>
        public async Task<Document> SingleOrDefaultAsync(Expression<Func<Document, bool>> predicate)
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 添加文档
        /// </summary>
        public async Task<Document> AddAsync(Document entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Documents.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// 批量添加文档
        /// </summary>
        public async Task<IEnumerable<Document>> AddRangeAsync(IEnumerable<Document> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var documentList = entities.ToList();
            _context.Documents.AddRange(documentList);
            await _context.SaveChangesAsync();
            return documentList;
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        public async Task<Document> UpdateAsync(Document entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        public async Task<bool> DeleteAsync(Document entity)
        {
            if (entity == null)
                return false;

            _context.Documents.Remove(entity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 根据ID删除文档
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Documents.FindAsync(id);
            if (entity == null)
                return false;

            _context.Documents.Remove(entity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 批量删除文档
        /// </summary>
        public async Task<int> DeleteRangeAsync(IEnumerable<Document> entities)
        {
            if (entities == null)
                return 0;

            var documentList = entities.ToList();
            _context.Documents.RemoveRange(documentList);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 计算文档数量
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.Documents.CountAsync();
        }

        /// <summary>
        /// 根据条件计算文档数量
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<Document, bool>> predicate)
        {
            return await _context.Documents.CountAsync(predicate);
        }

        /// <summary>
        /// 检查是否存在满足条件的文档
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<Document, bool>> predicate)
        {
            return await _context.Documents.AnyAsync(predicate);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PagedResult<Document>> GetPagedAsync<TKey>(
            int pageIndex, 
            int pageSize, 
            Expression<Func<Document, TKey>> orderBy, 
            bool ascending = true)
        {
            var query = _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template);

            if (ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);

            var totalCount = await query.CountAsync();
            var data = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Document>
            {
                Data = data,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 分页查询（带条件）
        /// </summary>
        public async Task<PagedResult<Document>> GetPagedAsync<TKey>(
            Expression<Func<Document, bool>> predicate,
            int pageIndex, 
            int pageSize, 
            Expression<Func<Document, TKey>> orderBy, 
            bool ascending = true)
        {
            var query = _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .Where(predicate);

            if (ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);

            var totalCount = await query.CountAsync();
            var data = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Document>
            {
                Data = data,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 根据关键词搜索文档
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>符合条件的文档列表</returns>
        public async Task<IEnumerable<Document>> SearchByKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .Where(d => d.Title.Contains(keyword) || 
                           d.Content.Contains(keyword) ||
                           d.SenderUnit.Contains(keyword) ||
                           d.DocumentNumber.Contains(keyword))
                .ToListAsync();
        }

        /// <summary>
        /// 获取待办文档
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>待办文档列表</returns>
        public async Task<IEnumerable<Document>> GetPendingDocumentsAsync(int userId)
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .Where(d => d.ResponsibleUserId == userId && 
                           (d.Status == "待办" || d.Status == "处理中"))
                .OrderByDescending(d => d.CreatedTime)
                .ToListAsync();
        }

        /// <summary>
        /// 获取部门文档
        /// </summary>
        /// <param name="departmentId">部门ID</param>
        /// <returns>部门文档列表</returns>
        public async Task<IEnumerable<Document>> GetDepartmentDocumentsAsync(int departmentId)
        {
            return await _context.Documents
                .Include(d => d.PrimaryDepartment)
                .Include(d => d.ResponsibleUser)
                .Include(d => d.CreatedUser)
                .Include(d => d.Template)
                .Where(d => d.PrimaryDepartmentId == departmentId ||
                           d.CopyDepartmentIds.Contains(departmentId.ToString()))
                .OrderByDescending(d => d.CreatedTime)
                .ToListAsync();
        }
    }
}