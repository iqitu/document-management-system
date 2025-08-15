using DocumentManagement.Core.Interfaces;
using DocumentManagement.Models;
using DocumentManagement.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace DocumentManagement.Services.Services
{
    /// <summary>
    /// 表单生成服务实现
    /// </summary>
    public class FormGenerationService : IFormGenerationService
    {
        private readonly IWordTemplateParser _templateParser;
        private readonly IDynamicFormGenerator _formGenerator;

        public FormGenerationService(IWordTemplateParser templateParser, IDynamicFormGenerator formGenerator)
        {
            _templateParser = templateParser ?? throw new ArgumentNullException(nameof(templateParser));
            _formGenerator = formGenerator ?? throw new ArgumentNullException(nameof(formGenerator));
        }

        public async Task<WpfFormDefinition> GenerateFormFromTemplateAsync(string templatePath)
        {
            return await Task.Run(() => _templateParser.ParseTemplate(templatePath));
        }

        public async Task<object> CreateWpfFormAsync(WpfFormDefinition formDefinition)
        {
            return await Task.Run(() => _formGenerator.GenerateForm(formDefinition));
        }

        public async Task BindDataToFormAsync(object form, DocumentRecord data)
        {
            await Task.Run(() => _formGenerator.ApplyDataBinding(form, data));
        }

        public async Task<DocumentRecord> ExtractDataFromFormAsync(object form)
        {
            // 从表单控件中提取数据并创建DocumentRecord
            // 这需要根据具体的WPF控件实现来确定如何提取值
            return await Task.Run(() => new DocumentRecord
            {
                // 暂时返回空记录，实际实现需要从表单控件读取值
                CreatedDate = DateTime.Now
            });
        }

        public async Task ValidateFormAsync(object form)
        {
            await Task.Run(() =>
            {
                // 执行表单验证逻辑
                // 检查必填字段、格式验证等
            });
        }
    }
}