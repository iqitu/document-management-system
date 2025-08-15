using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.XWPF.UserModel;
using DocumentManagement.Common;

namespace DocumentManagement.TemplateEngine
{
    /// <summary>
    /// Word模板解析器
    /// </summary>
    public class WordTemplateParser
    {
        /// <summary>
        /// 解析Word文档模板
        /// </summary>
        /// <param name="templatePath">模板文件路径</param>
        /// <returns>模板元数据</returns>
        public TemplateMetadata ParseTemplate(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                throw new FileNotFoundException($"模板文件不存在: {templatePath}");
            }

            var metadata = new TemplateMetadata
            {
                TemplatePath = templatePath,
                TemplateType = "Word",
                ParseTime = DateTime.Now,
                Fields = new List<TemplateField>()
            };

            try
            {
                using (var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    var document = new XWPFDocument(fileStream);
                    
                    // 解析段落中的字段
                    ParseParagraphs(document.Paragraphs, metadata);
                    
                    // 解析表格中的字段
                    ParseTables(document.Tables, metadata);
                    
                    // 解析页眉页脚
                    ParseHeadersAndFooters(document, metadata);
                    
                    metadata.IsValid = true;
                    metadata.Message = "模板解析成功";
                }
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = $"模板解析失败: {ex.Message}";
            }

            return metadata;
        }

        /// <summary>
        /// 解析段落中的字段
        /// </summary>
        /// <param name="paragraphs">段落列表</param>
        /// <param name="metadata">模板元数据</param>
        private void ParseParagraphs(IList<XWPFParagraph> paragraphs, TemplateMetadata metadata)
        {
            foreach (var paragraph in paragraphs)
            {
                var text = paragraph.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    ExtractFieldsFromText(text, metadata, "Paragraph");
                }
            }
        }

        /// <summary>
        /// 解析表格中的字段
        /// </summary>
        /// <param name="tables">表格列表</param>
        /// <param name="metadata">模板元数据</param>
        private void ParseTables(IList<XWPFTable> tables, TemplateMetadata metadata)
        {
            foreach (var table in tables)
            {
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.GetTableCells())
                    {
                        var text = cell.GetText();
                        if (!string.IsNullOrEmpty(text))
                        {
                            ExtractFieldsFromText(text, metadata, "Table");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析页眉页脚
        /// </summary>
        /// <param name="document">Word文档</param>
        /// <param name="metadata">模板元数据</param>
        private void ParseHeadersAndFooters(XWPFDocument document, TemplateMetadata metadata)
        {
            // 解析页眉
            foreach (var header in document.HeaderList)
            {
                foreach (var paragraph in header.Paragraphs)
                {
                    var text = paragraph.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        ExtractFieldsFromText(text, metadata, "Header");
                    }
                }
            }

            // 解析页脚
            foreach (var footer in document.FooterList)
            {
                foreach (var paragraph in footer.Paragraphs)
                {
                    var text = paragraph.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        ExtractFieldsFromText(text, metadata, "Footer");
                    }
                }
            }
        }

        /// <summary>
        /// 从文本中提取字段
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="metadata">模板元数据</param>
        /// <param name="location">位置</param>
        private void ExtractFieldsFromText(string text, TemplateMetadata metadata, string location)
        {
            // 查找 {字段名} 格式的占位符
            var fieldPattern = @"\{([^}]+)\}";
            var matches = System.Text.RegularExpressions.Regex.Matches(text, fieldPattern);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var fieldName = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(fieldName))
                {
                    var existingField = metadata.Fields.FirstOrDefault(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                    if (existingField == null)
                    {
                        var field = CreateTemplateField(fieldName, location);
                        metadata.Fields.Add(field);
                    }
                }
            }
        }

        /// <summary>
        /// 创建模板字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="location">位置</param>
        /// <returns>模板字段</returns>
        private TemplateField CreateTemplateField(string fieldName, string location)
        {
            var field = new TemplateField
            {
                Name = fieldName,
                DisplayName = GetFieldDisplayName(fieldName),
                FieldType = GetFieldType(fieldName),
                IsRequired = IsRequiredField(fieldName),
                Location = location,
                SortOrder = GetFieldSortOrder(fieldName)
            };

            // 根据字段类型设置属性
            switch (field.FieldType)
            {
                case "TextBox":
                    field.MaxLength = GetFieldMaxLength(fieldName);
                    break;
                case "ComboBox":
                    field.Options = GetFieldOptions(fieldName);
                    break;
                case "DatePicker":
                    field.DefaultValue = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
            }

            return field;
        }

        /// <summary>
        /// 获取字段显示名称
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>显示名称</returns>
        private string GetFieldDisplayName(string fieldName)
        {
            var displayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title", "文档标题" },
                { "DocumentNumber", "文档编号" },
                { "SenderUnit", "发文单位" },
                { "ReceivedDate", "收文日期" },
                { "UrgencyLevel", "紧急程度" },
                { "SecurityLevel", "密级" },
                { "Content", "文档内容" },
                { "PrimaryDepartment", "主送部门" },
                { "ResponsibleUser", "负责人" },
                { "DeadlineDate", "办理期限" },
                { "Remarks", "备注" }
            };

            return displayNames.TryGetValue(fieldName, out var displayName) ? displayName : fieldName;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>字段类型</returns>
        private string GetFieldType(string fieldName)
        {
            var fieldTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title", "TextBox" },
                { "DocumentNumber", "TextBox" },
                { "SenderUnit", "TextBox" },
                { "ReceivedDate", "DatePicker" },
                { "DeadlineDate", "DatePicker" },
                { "UrgencyLevel", "ComboBox" },
                { "SecurityLevel", "ComboBox" },
                { "Content", "RichTextBox" },
                { "PrimaryDepartment", "ComboBox" },
                { "ResponsibleUser", "ComboBox" },
                { "Remarks", "TextBox" }
            };

            return fieldTypes.TryGetValue(fieldName, out var fieldType) ? fieldType : "TextBox";
        }

        /// <summary>
        /// 判断是否为必填字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>是否必填</returns>
        private bool IsRequiredField(string fieldName)
        {
            var requiredFields = new[] { "Title", "DocumentNumber", "SenderUnit", "ReceivedDate", "Content" };
            return requiredFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取字段最大长度
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>最大长度</returns>
        private int? GetFieldMaxLength(string fieldName)
        {
            var maxLengths = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title", 200 },
                { "DocumentNumber", 50 },
                { "SenderUnit", 100 },
                { "UrgencyLevel", 20 },
                { "SecurityLevel", 20 },
                { "Remarks", 500 }
            };

            return maxLengths.TryGetValue(fieldName, out var maxLength) ? maxLength : (int?)null;
        }

        /// <summary>
        /// 获取字段选项
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>选项列表</returns>
        private List<string> GetFieldOptions(string fieldName)
        {
            switch (fieldName.ToLower())
            {
                case "urgencylevel":
                    return new List<string>
                    {
                        Constants.UrgencyLevel.Normal,
                        Constants.UrgencyLevel.Urgent,
                        Constants.UrgencyLevel.VeryUrgent,
                        Constants.UrgencyLevel.Emergency
                    };
                case "securitylevel":
                    return new List<string>
                    {
                        Constants.SecurityLevel.Public,
                        Constants.SecurityLevel.Internal,
                        Constants.SecurityLevel.Confidential,
                        Constants.SecurityLevel.Secret,
                        Constants.SecurityLevel.TopSecret
                    };
                default:
                    return new List<string>();
            }
        }

        /// <summary>
        /// 获取字段排序顺序
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>排序顺序</returns>
        private int GetFieldSortOrder(string fieldName)
        {
            var sortOrders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title", 1 },
                { "DocumentNumber", 2 },
                { "SenderUnit", 3 },
                { "ReceivedDate", 4 },
                { "UrgencyLevel", 5 },
                { "SecurityLevel", 6 },
                { "PrimaryDepartment", 7 },
                { "ResponsibleUser", 8 },
                { "Content", 9 },
                { "DeadlineDate", 10 },
                { "Remarks", 11 }
            };

            return sortOrders.TryGetValue(fieldName, out var sortOrder) ? sortOrder : 99;
        }
    }
}