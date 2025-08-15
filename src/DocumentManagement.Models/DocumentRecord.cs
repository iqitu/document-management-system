using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models
{
    /// <summary>
    /// 收文记录实体类
    /// </summary>
    public class DocumentRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; }    // 收文编号

        [Required]
        [StringLength(200)]
        public string Title { get; set; }             // 标题

        [StringLength(100)]
        public string SenderUnit { get; set; }        // 发文单位

        public DateTime ReceiveDate { get; set; }     // 收文日期

        public DocumentStatus Status { get; set; }    // 处理状态

        [StringLength(50)]
        public string Category { get; set; }          // 文件类别

        [StringLength(20)]
        public string Urgency { get; set; }           // 紧急程度

        [StringLength(50)]
        public string Handler { get; set; }           // 承办人

        [StringLength(50)]
        public string Department { get; set; }        // 承办部门

        public DateTime? Deadline { get; set; }       // 办理期限

        [StringLength(500)]
        public string Remarks { get; set; }           // 备注

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<DocumentAttachment> Attachments { get; set; } = new List<DocumentAttachment>();
    }

    /// <summary>
    /// 文档状态枚举
    /// </summary>
    public enum DocumentStatus
    {
        Pending = 0,      // 待处理
        Processing = 1,   // 处理中
        Completed = 2,    // 已完成
        Archived = 3      // 已归档
    }

    /// <summary>
    /// 文档附件
    /// </summary>
    public class DocumentAttachment
    {
        public int Id { get; set; }
        public int DocumentRecordId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }
        
        public long FileSize { get; set; }
        
        [StringLength(50)]
        public string ContentType { get; set; }
        
        public DateTime UploadDate { get; set; }

        public virtual DocumentRecord DocumentRecord { get; set; }
    }
}