using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Models;

/// <summary>
/// Word模板实体
/// </summary>
public class DocumentTemplate
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// 模板名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 模板描述
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 模板文件路径
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// 模板元数据（JSON格式）
    /// </summary>
    public string Metadata { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
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
/// 模板控件元数据
/// </summary>
public class TemplateControl
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// 所属模板ID
    /// </summary>
    public int TemplateId { get; set; }
    
    /// <summary>
    /// 控件名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 控件类型
    /// </summary>
    public ControlType Type { get; set; }
    
    /// <summary>
    /// 控件位置X
    /// </summary>
    public double X { get; set; }
    
    /// <summary>
    /// 控件位置Y
    /// </summary>
    public double Y { get; set; }
    
    /// <summary>
    /// 控件宽度
    /// </summary>
    public double Width { get; set; }
    
    /// <summary>
    /// 控件高度
    /// </summary>
    public double Height { get; set; }
    
    /// <summary>
    /// 控件属性（JSON格式）
    /// </summary>
    public string Properties { get; set; } = string.Empty;
    
    /// <summary>
    /// 导航属性
    /// </summary>
    public DocumentTemplate Template { get; set; } = null!;
}

/// <summary>
/// 控件类型枚举
/// </summary>
public enum ControlType
{
    /// <summary>
    /// 文本框
    /// </summary>
    TextBox = 1,
    
    /// <summary>
    /// 下拉列表
    /// </summary>
    ComboBox = 2,
    
    /// <summary>
    /// 日期选择器
    /// </summary>
    DatePicker = 3,
    
    /// <summary>
    /// 复选框
    /// </summary>
    CheckBox = 4,
    
    /// <summary>
    /// 单选按钮
    /// </summary>
    RadioButton = 5,
    
    /// <summary>
    /// 标签
    /// </summary>
    Label = 6,
    
    /// <summary>
    /// 表格
    /// </summary>
    DataGrid = 7
}