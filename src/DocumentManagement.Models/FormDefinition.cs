using System;
using System.Collections.Generic;

namespace DocumentManagement.Models
{
    /// <summary>
    /// WPF表单定义
    /// </summary>
    public class WpfFormDefinition
    {
        public string FormName { get; set; }
        public string Title { get; set; }
        public List<FormField> Fields { get; set; } = new List<FormField>();
        public StyleMapping Styles { get; set; } = new StyleMapping();
        public LayoutDefinition Layout { get; set; } = new LayoutDefinition();
    }

    /// <summary>
    /// 表单字段定义
    /// </summary>
    public class FormField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public FormFieldType FieldType { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public ValidationRules Validation { get; set; } = new ValidationRules();
        public FieldPosition Position { get; set; } = new FieldPosition();
        public string DataBinding { get; set; }
        public List<string> DependsOn { get; set; } = new List<string>();
    }

    /// <summary>
    /// 表单字段类型
    /// </summary>
    public enum FormFieldType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        RadioButton,
        MultiLineText,
        NumericUpDown,
        Label
    }

    /// <summary>
    /// 字段位置定义
    /// </summary>
    public class FieldPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;
        public double Width { get; set; }
        public double Height { get; set; }
    }

    /// <summary>
    /// 验证规则
    /// </summary>
    public class ValidationRules
    {
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 样式映射
    /// </summary>
    public class StyleMapping
    {
        public Dictionary<string, object> FontStyles { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Colors { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Margins { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Paddings { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// 布局定义
    /// </summary>
    public class LayoutDefinition
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<double> RowHeights { get; set; } = new List<double>();
        public List<double> ColumnWidths { get; set; } = new List<double>();
        public string LayoutType { get; set; } = "Grid"; // Grid, StackPanel, DockPanel, etc.
    }
}