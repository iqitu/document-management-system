using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentManagement.Models;
using DocumentManagement.Common;

namespace DocumentManagement.Core
{
    /// <summary>
    /// 模板解析器
    /// </summary>
    public class TemplateParser
    {
        /// <summary>
        /// 解析Word模板
        /// </summary>
        /// <param name="templatePath">模板文件路径</param>
        /// <returns>解析结果</returns>
        public TemplateParseResult ParseWordTemplate(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                throw new FileNotFoundException($"模板文件不存在: {templatePath}");
            }

            var result = new TemplateParseResult
            {
                TemplatePath = templatePath,
                IsValid = true,
                Fields = new List<TemplateField>(),
                ParseTime = DateTime.Now
            };

            try
            {
                // 这里是基础实现，实际项目中需要使用NPOI或其他库来解析Word文档
                result.Fields = ExtractFieldsFromTemplate(templatePath);
                result.Message = "模板解析成功";
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Message = $"模板解析失败: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 从模板中提取字段信息
        /// </summary>
        /// <param name="templatePath">模板路径</param>
        /// <returns>字段列表</returns>
        private List<TemplateField> ExtractFieldsFromTemplate(string templatePath)
        {
            var fields = new List<TemplateField>();

            // 示例字段，实际实现需要解析Word文档
            fields.Add(new TemplateField
            {
                Name = "Title",
                DisplayName = "文档标题",
                FieldType = "TextBox",
                IsRequired = true,
                MaxLength = 200,
                SortOrder = 1
            });

            fields.Add(new TemplateField
            {
                Name = "SenderUnit",
                DisplayName = "发文单位",
                FieldType = "TextBox",
                IsRequired = true,
                MaxLength = 100,
                SortOrder = 2
            });

            fields.Add(new TemplateField
            {
                Name = "ReceivedDate",
                DisplayName = "收文日期",
                FieldType = "DatePicker",
                IsRequired = true,
                SortOrder = 3
            });

            fields.Add(new TemplateField
            {
                Name = "UrgencyLevel",
                DisplayName = "紧急程度",
                FieldType = "ComboBox",
                IsRequired = false,
                Options = new List<string> 
                { 
                    Constants.UrgencyLevel.Normal,
                    Constants.UrgencyLevel.Urgent,
                    Constants.UrgencyLevel.VeryUrgent,
                    Constants.UrgencyLevel.Emergency
                },
                SortOrder = 4
            });

            fields.Add(new TemplateField
            {
                Name = "SecurityLevel",
                DisplayName = "密级",
                FieldType = "ComboBox",
                IsRequired = false,
                Options = new List<string>
                {
                    Constants.SecurityLevel.Public,
                    Constants.SecurityLevel.Internal,
                    Constants.SecurityLevel.Confidential,
                    Constants.SecurityLevel.Secret,
                    Constants.SecurityLevel.TopSecret
                },
                SortOrder = 5
            });

            fields.Add(new TemplateField
            {
                Name = "Content",
                DisplayName = "文档内容",
                FieldType = "RichTextBox",
                IsRequired = true,
                SortOrder = 6
            });

            return fields;
        }

        /// <summary>
        /// 验证模板字段
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns>验证结果</returns>
        public ValidationResult ValidateTemplateFields(List<TemplateField> fields)
        {
            var result = new ValidationResult { IsValid = true, Errors = new List<string>() };

            if (fields == null || !fields.Any())
            {
                result.IsValid = false;
                result.Errors.Add("模板字段不能为空");
                return result;
            }

            // 检查必填字段
            var titleField = fields.FirstOrDefault(f => f.Name.Equals("Title", StringComparison.OrdinalIgnoreCase));
            if (titleField == null)
            {
                result.IsValid = false;
                result.Errors.Add("缺少必要的标题字段");
            }

            // 检查字段名称唯一性
            var duplicateNames = fields.GroupBy(f => f.Name.ToLower())
                                     .Where(g => g.Count() > 1)
                                     .Select(g => g.Key);
            
            foreach (var duplicateName in duplicateNames)
            {
                result.IsValid = false;
                result.Errors.Add($"字段名称重复: {duplicateName}");
            }

            // 检查字段类型有效性
            var validFieldTypes = new[] { "TextBox", "RichTextBox", "ComboBox", "DatePicker", "CheckBox", "NumericUpDown" };
            foreach (var field in fields)
            {
                if (!validFieldTypes.Contains(field.FieldType))
                {
                    result.IsValid = false;
                    result.Errors.Add($"无效的字段类型: {field.FieldType}");
                }
            }

            return result;
        }

        /// <summary>
        /// 生成模板元数据
        /// </summary>
        /// <param name="templatePath">模板路径</param>
        /// <param name="fields">字段列表</param>
        /// <returns>模板元数据</returns>
        public string GenerateTemplateMetadata(string templatePath, List<TemplateField> fields)
        {
            var metadata = new
            {
                TemplatePath = templatePath,
                ParseTime = DateTime.Now,
                FieldCount = fields.Count,
                Fields = fields.Select(f => new
                {
                    f.Name,
                    f.DisplayName,
                    f.FieldType,
                    f.IsRequired,
                    f.MaxLength,
                    f.Options,
                    f.SortOrder
                }),
                Version = "1.0",
                Parser = "DocumentManagement.TemplateParser"
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented);
        }
    }

    /// <summary>
    /// 模板解析结果
    /// </summary>
    public class TemplateParseResult
    {
        public string TemplatePath { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public List<TemplateField> Fields { get; set; }
        public DateTime ParseTime { get; set; }
    }

    /// <summary>
    /// 模板字段
    /// </summary>
    public class TemplateField
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FieldType { get; set; }
        public bool IsRequired { get; set; }
        public int? MaxLength { get; set; }
        public List<string> Options { get; set; }
        public int SortOrder { get; set; }
        public string DefaultValue { get; set; }
        public string ValidationRule { get; set; }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
    }
}