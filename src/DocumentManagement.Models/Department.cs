using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models
{
    /// <summary>
    /// 部门数据模型
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 部门代码
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public int? ParentDepartmentId { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        public virtual Department ParentDepartment { get; set; }

        /// <summary>
        /// 部门级别
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 部门负责人ID
        /// </summary>
        public int? ManagerUserId { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public virtual User ManagerUser { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(50)]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 办公地址
        /// </summary>
        [StringLength(200)]
        public string OfficeAddress { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }
    }
}