using System;
using System.Collections.Generic;

namespace DocumentManagement.TemplateEngine
{
    /// <summary>
    /// 模板元数据
    /// </summary>
    public class TemplateMetadata
    {
        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 解析消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 解析时间
        /// </summary>
        public DateTime ParseTime { get; set; }

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<TemplateField> Fields { get; set; }

        /// <summary>
        /// 模板版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateMetadata()
        {
            Fields = new List<TemplateField>();
            IsValid = false;
            ParseTime = DateTime.Now;
            Version = "1.0";
        }

        /// <summary>
        /// 获取字段数量
        /// </summary>
        public int FieldCount => Fields?.Count ?? 0;

        /// <summary>
        /// 获取必填字段数量
        /// </summary>
        public int RequiredFieldCount => Fields?.FindAll(f => f.IsRequired).Count ?? 0;

        /// <summary>
        /// 根据名称获取字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>字段对象</returns>
        public TemplateField GetField(string fieldName)
        {
            return Fields?.Find(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="field">字段对象</param>
        public void AddField(TemplateField field)
        {
            if (field == null)
                return;

            if (Fields == null)
                Fields = new List<TemplateField>();

            // 检查是否已存在同名字段
            var existingField = GetField(field.Name);
            if (existingField == null)
            {
                Fields.Add(field);
            }
        }

        /// <summary>
        /// 移除字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveField(string fieldName)
        {
            var field = GetField(fieldName);
            if (field != null)
            {
                return Fields.Remove(field);
            }
            return false;
        }

        /// <summary>
        /// 验证模板元数据
        /// </summary>
        /// <returns>验证结果</returns>
        public ValidationResult Validate()
        {
            var result = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };

            // 检查模板路径
            if (string.IsNullOrEmpty(TemplatePath))
            {
                result.IsValid = false;
                result.Errors.Add("模板路径不能为空");
            }

            // 检查字段
            if (Fields == null || Fields.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("模板必须包含至少一个字段");
            }
            else
            {
                // 检查字段名称唯一性
                var fieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var field in Fields)
                {
                    if (string.IsNullOrEmpty(field.Name))
                    {
                        result.IsValid = false;
                        result.Errors.Add("字段名称不能为空");
                    }
                    else if (!fieldNames.Add(field.Name))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"字段名称重复: {field.Name}");
                    }
                }

                // 检查必要字段
                var hasTitle = Fields.Exists(f => f.Name.Equals("Title", StringComparison.OrdinalIgnoreCase));
                if (!hasTitle)
                {
                    result.IsValid = false;
                    result.Errors.Add("模板必须包含标题字段");
                }
            }

            return result;
        }

        /// <summary>
        /// 转换为JSON字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        public string ToJson()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从JSON字符串创建实例
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>模板元数据实例</returns>
        public static TemplateMetadata FromJson(string json)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateMetadata>(json);
            }
            catch (Exception)
            {
                return new TemplateMetadata();
            }
        }
    }

    /// <summary>
    /// 模板字段
    /// </summary>
    public class TemplateField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 验证规则
        /// </summary>
        public string ValidationRule { get; set; }

        /// <summary>
        /// 字段位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateField()
        {
            IsVisible = true;
            IsReadOnly = false;
            SortOrder = 99;
            FieldType = "TextBox";
        }

        /// <summary>
        /// 验证字段值
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns>验证结果</returns>
        public ValidationResult ValidateValue(object value)
        {
            var result = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };

            var stringValue = value?.ToString() ?? string.Empty;

            // 必填验证
            if (IsRequired && string.IsNullOrWhiteSpace(stringValue))
            {
                result.IsValid = false;
                result.Errors.Add($"{DisplayName ?? Name}为必填项");
                return result;
            }

            // 长度验证
            if (MaxLength.HasValue && stringValue.Length > MaxLength.Value)
            {
                result.IsValid = false;
                result.Errors.Add($"{DisplayName ?? Name}长度不能超过{MaxLength.Value}个字符");
            }

            // 选项验证
            if (Options != null && Options.Count > 0 && !string.IsNullOrEmpty(stringValue))
            {
                if (!Options.Contains(stringValue))
                {
                    result.IsValid = false;
                    result.Errors.Add($"{DisplayName ?? Name}的值不在允许的选项中");
                }
            }

            // 类型验证
            switch (FieldType)
            {
                case "DatePicker":
                    if (!string.IsNullOrEmpty(stringValue) && !DateTime.TryParse(stringValue, out _))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"{DisplayName ?? Name}不是有效的日期格式");
                    }
                    break;
                case "NumericUpDown":
                    if (!string.IsNullOrEmpty(stringValue) && !double.TryParse(stringValue, out _))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"{DisplayName ?? Name}不是有效的数字格式");
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// 克隆字段
        /// </summary>
        /// <returns>字段副本</returns>
        public TemplateField Clone()
        {
            return new TemplateField
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                FieldType = this.FieldType,
                IsRequired = this.IsRequired,
                MaxLength = this.MaxLength,
                Options = this.Options != null ? new List<string>(this.Options) : null,
                SortOrder = this.SortOrder,
                DefaultValue = this.DefaultValue,
                ValidationRule = this.ValidationRule,
                Location = this.Location,
                Description = this.Description,
                IsReadOnly = this.IsReadOnly,
                IsVisible = this.IsVisible
            };
        }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误列表
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValidationResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="error">错误信息</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                Errors.Add(error);
                IsValid = false;
            }
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        /// <returns>错误消息字符串</returns>
        public string GetErrorMessage()
        {
            return string.Join("\n", Errors);
        }
    }
}