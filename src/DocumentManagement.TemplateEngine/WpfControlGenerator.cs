using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace DocumentManagement.TemplateEngine
{
    /// <summary>
    /// WPF控件生成器
    /// </summary>
    public class WpfControlGenerator
    {
        /// <summary>
        /// 根据模板字段生成WPF控件
        /// </summary>
        /// <param name="fields">模板字段列表</param>
        /// <returns>生成的控件列表</returns>
        public List<UIElement> GenerateControls(List<TemplateField> fields)
        {
            if (fields == null || !fields.Any())
                return new List<UIElement>();

            var controls = new List<UIElement>();
            var sortedFields = fields.OrderBy(f => f.SortOrder).ToList();

            foreach (var field in sortedFields)
            {
                var control = CreateControlForField(field);
                if (control != null)
                {
                    controls.Add(control);
                }
            }

            return controls;
        }

        /// <summary>
        /// 为字段创建对应的WPF控件
        /// </summary>
        /// <param name="field">模板字段</param>
        /// <returns>WPF控件</returns>
        private UIElement CreateControlForField(TemplateField field)
        {
            switch (field.FieldType)
            {
                case "TextBox":
                    return CreateTextBox(field);
                case "RichTextBox":
                    return CreateRichTextBox(field);
                case "ComboBox":
                    return CreateComboBox(field);
                case "DatePicker":
                    return CreateDatePicker(field);
                case "CheckBox":
                    return CreateCheckBox(field);
                case "NumericUpDown":
                    return CreateNumericUpDown(field);
                default:
                    return CreateTextBox(field);
            }
        }

        /// <summary>
        /// 创建文本框控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>文本框控件</returns>
        private StackPanel CreateTextBox(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 标签
            var label = new Label
            {
                Content = field.DisplayName + (field.IsRequired ? " *" : ""),
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };
            panel.Children.Add(label);

            // 文本框
            var textBox = new TextBox
            {
                Name = field.Name,
                MaxLength = field.MaxLength ?? 0,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Top
            };

            if (!string.IsNullOrEmpty(field.DefaultValue))
            {
                textBox.Text = field.DefaultValue;
            }

            panel.Children.Add(textBox);

            return panel;
        }

        /// <summary>
        /// 创建富文本框控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>富文本框控件</returns>
        private StackPanel CreateRichTextBox(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 标签
            var label = new Label
            {
                Content = field.DisplayName + (field.IsRequired ? " *" : ""),
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };
            panel.Children.Add(label);

            // 富文本框
            var richTextBox = new RichTextBox
            {
                Name = field.Name,
                Height = 100,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                AcceptsReturn = true,
                AcceptsTab = true
            };

            panel.Children.Add(richTextBox);

            return panel;
        }

        /// <summary>
        /// 创建下拉框控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>下拉框控件</returns>
        private StackPanel CreateComboBox(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 标签
            var label = new Label
            {
                Content = field.DisplayName + (field.IsRequired ? " *" : ""),
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };
            panel.Children.Add(label);

            // 下拉框
            var comboBox = new ComboBox
            {
                Name = field.Name,
                Height = 25,
                IsEditable = false
            };

            // 添加选项
            if (field.Options != null && field.Options.Any())
            {
                foreach (var option in field.Options)
                {
                    comboBox.Items.Add(option);
                }
            }

            // 设置默认值
            if (!string.IsNullOrEmpty(field.DefaultValue) && comboBox.Items.Contains(field.DefaultValue))
            {
                comboBox.SelectedItem = field.DefaultValue;
            }

            panel.Children.Add(comboBox);

            return panel;
        }

        /// <summary>
        /// 创建日期选择器控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>日期选择器控件</returns>
        private StackPanel CreateDatePicker(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 标签
            var label = new Label
            {
                Content = field.DisplayName + (field.IsRequired ? " *" : ""),
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };
            panel.Children.Add(label);

            // 日期选择器
            var datePicker = new DatePicker
            {
                Name = field.Name,
                Height = 25,
                SelectedDateFormat = DatePickerFormat.Short
            };

            // 设置默认值
            if (!string.IsNullOrEmpty(field.DefaultValue) && DateTime.TryParse(field.DefaultValue, out var defaultDate))
            {
                datePicker.SelectedDate = defaultDate;
            }

            panel.Children.Add(datePicker);

            return panel;
        }

        /// <summary>
        /// 创建复选框控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>复选框控件</returns>
        private StackPanel CreateCheckBox(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 复选框
            var checkBox = new CheckBox
            {
                Name = field.Name,
                Content = field.DisplayName,
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };

            // 设置默认值
            if (!string.IsNullOrEmpty(field.DefaultValue) && bool.TryParse(field.DefaultValue, out var defaultValue))
            {
                checkBox.IsChecked = defaultValue;
            }

            panel.Children.Add(checkBox);

            return panel;
        }

        /// <summary>
        /// 创建数字输入框控件
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns>数字输入框控件</returns>
        private StackPanel CreateNumericUpDown(TemplateField field)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // 标签
            var label = new Label
            {
                Content = field.DisplayName + (field.IsRequired ? " *" : ""),
                FontWeight = field.IsRequired ? FontWeights.Bold : FontWeights.Normal
            };
            panel.Children.Add(label);

            // 数字输入框（使用TextBox模拟）
            var textBox = new TextBox
            {
                Name = field.Name,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Top
            };

            // 设置默认值
            if (!string.IsNullOrEmpty(field.DefaultValue))
            {
                textBox.Text = field.DefaultValue;
            }

            panel.Children.Add(textBox);

            return panel;
        }

        /// <summary>
        /// 生成表单布局
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <param name="columnsCount">列数</param>
        /// <returns>表单网格</returns>
        public Grid GenerateFormLayout(List<TemplateField> fields, int columnsCount = 2)
        {
            if (fields == null || !fields.Any())
                return new Grid();

            var grid = new Grid();
            var sortedFields = fields.OrderBy(f => f.SortOrder).ToList();

            // 创建列定义
            for (int i = 0; i < columnsCount; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // 创建行定义
            var rowsCount = (int)Math.Ceiling((double)sortedFields.Count / columnsCount);
            for (int i = 0; i < rowsCount; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // 添加控件到网格
            for (int i = 0; i < sortedFields.Count; i++)
            {
                var field = sortedFields[i];
                var control = CreateControlForField(field);
                
                if (control != null)
                {
                    var row = i / columnsCount;
                    var column = i % columnsCount;

                    Grid.SetRow(control, row);
                    Grid.SetColumn(control, column);
                    control.Margin = new Thickness(5);

                    grid.Children.Add(control);
                }
            }

            return grid;
        }

        /// <summary>
        /// 从控件中获取数据
        /// </summary>
        /// <param name="container">控件容器</param>
        /// <returns>字段数据字典</returns>
        public Dictionary<string, object> GetFormData(DependencyObject container)
        {
            var data = new Dictionary<string, object>();

            var controls = GetChildControls(container);
            foreach (var control in controls)
            {
                var name = control.GetValue(FrameworkElement.NameProperty) as string;
                if (string.IsNullOrEmpty(name))
                    continue;

                object value = null;

                if (control is TextBox textBox)
                {
                    value = textBox.Text;
                }
                else if (control is ComboBox comboBox)
                {
                    value = comboBox.SelectedItem;
                }
                else if (control is DatePicker datePicker)
                {
                    value = datePicker.SelectedDate;
                }
                else if (control is CheckBox checkBox)
                {
                    value = checkBox.IsChecked;
                }
                else if (control is RichTextBox richTextBox)
                {
                    value = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
                }

                if (value != null)
                {
                    data[name] = value;
                }
            }

            return data;
        }

        /// <summary>
        /// 获取子控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <returns>子控件列表</returns>
        private List<FrameworkElement> GetChildControls(DependencyObject parent)
        {
            var controls = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is FrameworkElement frameworkElement && !string.IsNullOrEmpty(frameworkElement.Name))
                {
                    controls.Add(frameworkElement);
                }

                controls.AddRange(GetChildControls(child));
            }

            return controls;
        }
    }
}