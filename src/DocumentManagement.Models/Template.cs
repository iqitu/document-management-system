using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models
{
    /// <summary>
    /// 模板数据模型
    /// </summary>
    public class Template
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        [StringLength(50)]
        public string TemplateType { get; set; }

        /// <summary>
        /// 模板版本
        /// </summary>
        [StringLength(20)]
        public string Version { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

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
        /// 模板元数据（JSON格式）
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// 字段配置（JSON格式）
        /// </summary>
        public string FieldConfiguration { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }
    }
}