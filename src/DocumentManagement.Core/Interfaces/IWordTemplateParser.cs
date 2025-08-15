using DocumentFormat.OpenXml.Packaging;
using DocumentManagement.Models;
using System.Collections.Generic;

namespace DocumentManagement.Core.Interfaces
{
    /// <summary>
    /// Word模板解析器接口
    /// </summary>
    public interface IWordTemplateParser
    {
        /// <summary>
        /// 解析Word模板文件
        /// </summary>
        /// <param name="wordFilePath">Word文件路径</param>
        /// <returns>WPF表单定义</returns>
        WpfFormDefinition ParseTemplate(string wordFilePath);

        /// <summary>
        /// 从Word文档中提取表单字段
        /// </summary>
        /// <param name="document">Word文档</param>
        /// <returns>表单字段列表</returns>
        List<FormField> ExtractFormFields(WordprocessingDocument document);

        /// <summary>
        /// 转换Word样式
        /// </summary>
        /// <param name="document">Word文档</param>
        /// <returns>样式映射</returns>
        StyleMapping ConvertWordStyles(WordprocessingDocument document);
    }

    /// <summary>
    /// 动态表单生成器接口
    /// </summary>
    public interface IDynamicFormGenerator
    {
        /// <summary>
        /// 生成WPF表单
        /// </summary>
        /// <param name="formDefinition">表单定义</param>
        /// <returns>用户控件</returns>
        object GenerateForm(WpfFormDefinition formDefinition);

        /// <summary>
        /// 应用数据绑定
        /// </summary>
        /// <param name="form">表单控件</param>
        /// <param name="dataContext">数据上下文</param>
        void ApplyDataBinding(object form, object dataContext);

        /// <summary>
        /// 设置字段验证
        /// </summary>
        /// <param name="form">表单控件</param>
        /// <param name="rules">验证规则</param>
        void SetupFieldValidation(object form, ValidationRules rules);
    }
}