using DocumentManagement.Core.Interfaces;
using DocumentManagement.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DocumentManagement.WordParser.FormGenerator
{
    /// <summary>
    /// 动态WPF表单生成器实现
    /// </summary>
    public class DynamicFormGenerator : IDynamicFormGenerator
    {
        public object GenerateForm(WpfFormDefinition formDefinition)
        {
            if (formDefinition == null)
                throw new ArgumentNullException(nameof(formDefinition));

            // 创建主容器
            var grid = new Grid();
            
            // 设置网格行列定义
            SetupGridLayout(grid, formDefinition.Layout);
            
            // 生成表单字段
            foreach (var field in formDefinition.Fields)
            {
                var control = CreateFieldControl(field);
                if (control != null)
                {
                    Grid.SetRow(control, field.Position.Row);
                    Grid.SetColumn(control, field.Position.Column);
                    Grid.SetRowSpan(control, field.Position.RowSpan);
                    Grid.SetColumnSpan(control, field.Position.ColumnSpan);
                    
                    grid.Children.Add(control);
                }
            }

            // 创建用户控件包装器
            var userControl = new UserControl
            {
                Content = grid
            };

            return userControl;
        }

        public void ApplyDataBinding(object form, object dataContext)
        {
            if (form is UserControl userControl)
            {
                userControl.DataContext = dataContext;
                
                // 为子控件设置数据绑定
                if (userControl.Content is Grid grid)
                {
                    ApplyDataBindingToChildren(grid, dataContext);
                }
            }
        }

        public void SetupFieldValidation(object form, ValidationRules rules)
        {
            // 实现字段验证逻辑
            if (form is UserControl userControl && userControl.Content is Grid grid)
            {
                foreach (UIElement child in grid.Children)
                {
                    if (child is TextBox textBox)
                    {
                        SetupTextBoxValidation(textBox, rules);
                    }
                }
            }
        }

        private void SetupGridLayout(Grid grid, LayoutDefinition layout)
        {
            // 创建行定义
            for (int i = 0; i < layout.Rows; i++)
            {
                var rowDefinition = new RowDefinition();
                if (layout.RowHeights.Count > i)
                {
                    rowDefinition.Height = new GridLength(layout.RowHeights[i]);
                }
                else
                {
                    rowDefinition.Height = GridLength.Auto;
                }
                grid.RowDefinitions.Add(rowDefinition);
            }

            // 创建列定义
            for (int i = 0; i < layout.Columns; i++)
            {
                var columnDefinition = new ColumnDefinition();
                if (layout.ColumnWidths.Count > i)
                {
                    columnDefinition.Width = new GridLength(layout.ColumnWidths[i]);
                }
                else
                {
                    columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                }
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private FrameworkElement CreateFieldControl(FormField field)
        {
            switch (field.FieldType)
            {
                case FormFieldType.TextBox:
                    return CreateTextBox(field);
                
                case FormFieldType.MultiLineText:
                    return CreateMultiLineTextBox(field);
                
                case FormFieldType.ComboBox:
                    return CreateComboBox(field);
                
                case FormFieldType.DatePicker:
                    return CreateDatePicker(field);
                
                case FormFieldType.CheckBox:
                    return CreateCheckBox(field);
                
                case FormFieldType.Label:
                    return CreateLabel(field);
                
                default:
                    return CreateTextBox(field);
            }
        }

        private TextBox CreateTextBox(FormField field)
        {
            var textBox = new TextBox
            {
                Name = field.Name,
                Text = field.DefaultValue ?? "",
                MaxLength = field.MaxLength > 0 ? field.MaxLength : 0,
                Margin = new Thickness(2),
                Padding = new Thickness(4),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 设置数据绑定
            if (!string.IsNullOrEmpty(field.DataBinding))
            {
                var binding = new Binding(field.DataBinding)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                textBox.SetBinding(TextBox.TextProperty, binding);
            }

            return textBox;
        }

        private TextBox CreateMultiLineTextBox(FormField field)
        {
            var textBox = CreateTextBox(field);
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.Height = 60;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            
            return textBox;
        }

        private ComboBox CreateComboBox(FormField field)
        {
            var comboBox = new ComboBox
            {
                Name = field.Name,
                Margin = new Thickness(2),
                Padding = new Thickness(4),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 添加选项
            foreach (var option in field.Options)
            {
                comboBox.Items.Add(option);
            }

            // 设置默认值
            if (!string.IsNullOrEmpty(field.DefaultValue))
            {
                comboBox.SelectedItem = field.DefaultValue;
            }

            // 设置数据绑定
            if (!string.IsNullOrEmpty(field.DataBinding))
            {
                var binding = new Binding(field.DataBinding)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                comboBox.SetBinding(ComboBox.SelectedValueProperty, binding);
            }

            return comboBox;
        }

        private DatePicker CreateDatePicker(FormField field)
        {
            var datePicker = new DatePicker
            {
                Name = field.Name,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 设置默认值
            if (DateTime.TryParse(field.DefaultValue, out DateTime defaultDate))
            {
                datePicker.SelectedDate = defaultDate;
            }

            // 设置数据绑定
            if (!string.IsNullOrEmpty(field.DataBinding))
            {
                var binding = new Binding(field.DataBinding)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                datePicker.SetBinding(DatePicker.SelectedDateProperty, binding);
            }

            return datePicker;
        }

        private CheckBox CreateCheckBox(FormField field)
        {
            var checkBox = new CheckBox
            {
                Name = field.Name,
                Content = field.Label,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };

            // 设置默认值
            if (bool.TryParse(field.DefaultValue, out bool defaultValue))
            {
                checkBox.IsChecked = defaultValue;
            }

            // 设置数据绑定
            if (!string.IsNullOrEmpty(field.DataBinding))
            {
                var binding = new Binding(field.DataBinding)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
            }

            return checkBox;
        }

        private Label CreateLabel(FormField field)
        {
            var label = new Label
            {
                Name = field.Name,
                Content = field.Label,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center
            };

            return label;
        }

        private void ApplyDataBindingToChildren(Grid grid, object dataContext)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is FrameworkElement element)
                {
                    element.DataContext = dataContext;
                }
            }
        }

        private void SetupTextBoxValidation(TextBox textBox, ValidationRules rules)
        {
            if (rules.IsRequired)
            {
                // 添加必填验证
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    // 这里可以添加验证规则
                    // 实际实现需要创建自定义验证规则类
                }
            }
        }
    }
}