using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models;

/// <summary>
/// 收文信息实体
/// </summary>
public class Document
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// 收文编号
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// 文件标题
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 发文单位
    /// </summary>
    [MaxLength(100)]
    public string Sender { get; set; } = string.Empty;
    
    /// <summary>
    /// 收文日期
    /// </summary>
    public DateTime ReceivedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 文件内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 处理状态
    /// </summary>
    public DocumentStatus Status { get; set; } = DocumentStatus.Received;
    
    /// <summary>
    /// 处理人员
    /// </summary>
    [MaxLength(50)]
    public string Handler { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 收文状态枚举
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// 已接收
    /// </summary>
    Received = 1,
    
    /// <summary>
    /// 处理中
    /// </summary>
    Processing = 2,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// 已归档
    /// </summary>
    Archived = 4
}