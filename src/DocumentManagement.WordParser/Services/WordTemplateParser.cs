using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentManagement.Core.Interfaces;
using DocumentManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentManagement.WordParser.Services
{
    /// <summary>
    /// Word模板解析器实现
    /// </summary>
    public class WordTemplateParser : IWordTemplateParser
    {
        public WpfFormDefinition ParseTemplate(string wordFilePath)
        {
            if (!File.Exists(wordFilePath))
                throw new FileNotFoundException($"Word文件不存在: {wordFilePath}");

            var formDefinition = new WpfFormDefinition
            {
                FormName = Path.GetFileNameWithoutExtension(wordFilePath),
                Title = "收文登记表"
            };

            using (var document = WordprocessingDocument.Open(wordFilePath, false))
            {
                formDefinition.Fields = ExtractFormFields(document);
                formDefinition.Styles = ConvertWordStyles(document);
                formDefinition.Layout = DetermineLayout(formDefinition.Fields);
            }

            return formDefinition;
        }

        public List<FormField> ExtractFormFields(WordprocessingDocument document)
        {
            var fields = new List<FormField>();
            var body = document.MainDocumentPart.Document.Body;

            // 提取表格中的字段
            ExtractTableFields(body, fields);

            // 提取文本框字段  
            ExtractTextBoxFields(body, fields);

            // 提取内容控件字段
            ExtractContentControlFields(body, fields);

            return fields;
        }

        public StyleMapping ConvertWordStyles(WordprocessingDocument document)
        {
            var styleMapping = new StyleMapping();

            var stylePart = document.MainDocumentPart.StyleDefinitionsPart;
            if (stylePart?.Styles != null)
            {
                foreach (var style in stylePart.Styles.Elements<Style>())
                {
                    if (style.Type == StyleValues.Paragraph)
                    {
                        ExtractParagraphStyles(style, styleMapping);
                    }
                }
            }

            return styleMapping;
        }

        private void ExtractTableFields(Body body, List<FormField> fields)
        {
            var tables = body.Elements<Table>();
            int row = 0;

            foreach (var table in tables)
            {
                foreach (var tableRow in table.Elements<TableRow>())
                {
                    int col = 0;
                    foreach (var cell in tableRow.Elements<TableCell>())
                    {
                        var cellText = cell.InnerText.Trim();
                        if (!string.IsNullOrEmpty(cellText) && IsFieldLabel(cellText))
                        {
                            var field = CreateFieldFromLabel(cellText, row, col);
                            fields.Add(field);
                        }
                        col++;
                    }
                    row++;
                }
            }
        }

        private void ExtractTextBoxFields(Body body, List<FormField> fields)
        {
            // 在实际实现中，这里会查找文本框控件
            // 当前提供基础实现
        }

        private void ExtractContentControlFields(Body body, List<FormField> fields)
        {
            var contentControls = body.Descendants<SdtElement>();
            int index = 0;

            foreach (var control in contentControls)
            {
                var properties = control.Elements<SdtProperties>().FirstOrDefault();
                if (properties != null)
                {
                    var alias = properties.Elements<SdtAlias>().FirstOrDefault();
                    var tag = properties.Elements<Tag>().FirstOrDefault();
                    
                    var field = new FormField
                    {
                        Name = tag?.Val ?? $"Field_{index}",
                        Label = alias?.Val ?? $"字段 {index + 1}",
                        FieldType = DetermineFieldType(properties),
                        Position = new FieldPosition { Row = index / 2, Column = index % 2 }
                    };

                    fields.Add(field);
                    index++;
                }
            }
        }

        private FormField CreateFieldFromLabel(string label, int row, int col)
        {
            var fieldName = GenerateFieldName(label);
            var fieldType = DetermineFieldTypeFromLabel(label);

            return new FormField
            {
                Name = fieldName,
                Label = label.Replace("：", "").Replace(":", "").Trim(),
                FieldType = fieldType,
                Position = new FieldPosition { Row = row, Column = col },
                IsRequired = IsRequiredField(label)
            };
        }

        private FormFieldType DetermineFieldType(SdtProperties properties)
        {
            if (properties.Descendants().Any(e => e.LocalName == "comboBox"))
                return FormFieldType.ComboBox;
            if (properties.Descendants().Any(e => e.LocalName == "date"))
                return FormFieldType.DatePicker;
            
            return FormFieldType.TextBox;
        }

        private FormFieldType DetermineFieldTypeFromLabel(string label)
        {
            label = label.ToLower();
            
            if (label.Contains("日期") || label.Contains("时间"))
                return FormFieldType.DatePicker;
            if (label.Contains("状态") || label.Contains("类别") || label.Contains("级别"))
                return FormFieldType.ComboBox;
            if (label.Contains("备注") || label.Contains("说明"))
                return FormFieldType.MultiLineText;
            
            return FormFieldType.TextBox;
        }

        private string GenerateFieldName(string label)
        {
            var cleanLabel = label.Replace("：", "").Replace(":", "").Trim();
            
            // 映射中文标签到英文字段名
            var fieldMappings = new Dictionary<string, string>
            {
                { "收文编号", "DocumentNumber" },
                { "标题", "Title" },
                { "发文单位", "SenderUnit" },
                { "收文日期", "ReceiveDate" },
                { "处理状态", "Status" },
                { "文件类别", "Category" },
                { "紧急程度", "Urgency" },
                { "承办人", "Handler" },
                { "承办部门", "Department" },
                { "办理期限", "Deadline" },
                { "备注", "Remarks" }
            };

            return fieldMappings.ContainsKey(cleanLabel) 
                ? fieldMappings[cleanLabel] 
                : cleanLabel.Replace(" ", "");
        }

        private bool IsFieldLabel(string text)
        {
            return text.EndsWith("：") || text.EndsWith(":") || 
                   text.Contains("编号") || text.Contains("日期") || 
                   text.Contains("单位") || text.Contains("状态");
        }

        private bool IsRequiredField(string label)
        {
            var requiredLabels = new[] { "收文编号", "标题", "发文单位", "收文日期" };
            return requiredLabels.Any(req => label.Contains(req));
        }

        private LayoutDefinition DetermineLayout(List<FormField> fields)
        {
            int maxRow = fields.Any() ? fields.Max(f => f.Position.Row) + 1 : 1;
            int maxCol = fields.Any() ? fields.Max(f => f.Position.Column) + 1 : 2;

            return new LayoutDefinition
            {
                Rows = Math.Max(maxRow, 6),
                Columns = Math.Max(maxCol, 2),
                LayoutType = "Grid"
            };
        }

        private void ExtractParagraphStyles(Style style, StyleMapping styleMapping)
        {
            var paragraphProperties = style.Elements<StyleParagraphProperties>().FirstOrDefault();
            var runProperties = style.Elements<StyleRunProperties>().FirstOrDefault();

            if (runProperties != null)
            {
                var fontSize = runProperties.Elements<FontSize>().FirstOrDefault();
                if (fontSize != null)
                {
                    styleMapping.FontStyles[style.StyleId + "_FontSize"] = fontSize.Val;
                }
            }
        }
    }
}