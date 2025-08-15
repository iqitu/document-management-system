using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models
{
    /// <summary>
    /// 收文文档数据模型
    /// </summary>
    public class Document
    {
        /// <summary>
        /// 文档ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文档标题
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 文档编号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// 发文单位
        /// </summary>
        [StringLength(100)]
        public string SenderUnit { get; set; }

        /// <summary>
        /// 收文日期
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        [StringLength(20)]
        public string UrgencyLevel { get; set; }

        /// <summary>
        /// 密级
        /// </summary>
        [StringLength(20)]
        public string SecurityLevel { get; set; }

        /// <summary>
        /// 主送部门ID
        /// </summary>
        public int? PrimaryDepartmentId { get; set; }

        /// <summary>
        /// 主送部门
        /// </summary>
        public virtual Department PrimaryDepartment { get; set; }

        /// <summary>
        /// 抄送部门列表（以逗号分隔的部门ID）
        /// </summary>
        [StringLength(500)]
        public string CopyDepartmentIds { get; set; }

        /// <summary>
        /// 负责人ID
        /// </summary>
        public int? ResponsibleUserId { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public virtual User ResponsibleUser { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        [StringLength(500)]
        public string AttachmentPath { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; }

        /// <summary>
        /// 办理期限
        /// </summary>
        public DateTime? DeadlineDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public int? CreatedUserId { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int? TemplateId { get; set; }

        /// <summary>
        /// 关联模板
        /// </summary>
        public virtual Template Template { get; set; }
    }
}