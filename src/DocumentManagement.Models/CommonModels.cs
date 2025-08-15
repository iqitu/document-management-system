using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models
{
    /// <summary>
    /// 系统配置设置
    /// </summary>
    public class AppSettings
    {
        [StringLength(255)]
        public string DefaultTemplatesPath { get; set; } = "Templates";

        [StringLength(255)]
        public string AttachmentsPath { get; set; } = "Attachments";

        [StringLength(255)]
        public string BackupPath { get; set; } = "Backups";

        public int BackupRetentionDays { get; set; } = 30;

        public bool AutoBackupEnabled { get; set; } = true;

        public string Theme { get; set; } = "Light";

        public string Language { get; set; } = "zh-CN";
    }

    /// <summary>
    /// 搜索查询参数
    /// </summary>
    public class DocumentSearchCriteria
    {
        public string SearchText { get; set; }
        public DocumentStatus? Status { get; set; }
        public string Category { get; set; }
        public string SenderUnit { get; set; }
        public string Handler { get; set; }
        public string Department { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Urgency { get; set; }
    }

    /// <summary>
    /// 分页查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static OperationResult<T> SuccessResult(T data, string message = null)
        {
            return new OperationResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static OperationResult<T> FailureResult(string message, List<string> errors = null)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}