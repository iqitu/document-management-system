using DocumentManagement.Core.Services;
using DocumentManagement.WordParser;
using DocumentManagement.Models;

namespace DocumentManagement.Services;

/// <summary>
/// 文档处理服务接口
/// </summary>
public interface IDocumentProcessingService
{
    Task<Document> ProcessIncomingDocumentAsync(string title, string sender, string filePath, int? templateId = null);
    Task<DocumentTemplate> ImportWordTemplateAsync(string filePath, string templateName);
    Task<IEnumerable<TemplateControl>> GenerateWpfControlsAsync(int templateId);
    Task<Document> UpdateDocumentFromTemplateAsync(int documentId, Dictionary<string, object> fieldValues);
}

/// <summary>
/// 文档处理服务实现
/// </summary>
public class DocumentProcessingService : IDocumentProcessingService
{
    private readonly IDocumentService _documentService;
    private readonly ITemplateService _templateService;
    private readonly IWordDocumentParser _wordParser;

    public DocumentProcessingService(
        IDocumentService documentService,
        ITemplateService templateService,
        IWordDocumentParser wordParser)
    {
        _documentService = documentService;
        _templateService = templateService;
        _wordParser = wordParser;
    }

    public async Task<Document> ProcessIncomingDocumentAsync(string title, string sender, string filePath, int? templateId = null)
    {
        // 生成收文编号
        var documentNumber = GenerateDocumentNumber();
        
        // 提取文档内容
        var content = string.Empty;
        if (!string.IsNullOrWhiteSpace(filePath) && _wordParser.IsWordDocument(filePath))
        {
            content = await _wordParser.ExtractTextFromWordAsync(filePath);
        }

        var document = new Document
        {
            DocumentNumber = documentNumber,
            Title = title,
            Sender = sender,
            Content = content,
            ReceivedDate = DateTime.Now,
            Status = DocumentStatus.Received
        };

        return await _documentService.CreateDocumentAsync(document);
    }

    public async Task<DocumentTemplate> ImportWordTemplateAsync(string filePath, string templateName)
    {
        if (!_wordParser.IsWordDocument(filePath))
        {
            throw new ArgumentException("文件不是有效的Word文档", nameof(filePath));
        }

        // 解析Word模板
        var template = await _wordParser.ParseWordTemplateAsync(filePath, templateName);
        
        // 保存到数据库
        return await _templateService.CreateTemplateAsync(template);
    }

    public async Task<IEnumerable<TemplateControl>> GenerateWpfControlsAsync(int templateId)
    {
        var template = await _templateService.GetTemplateWithControlsAsync(templateId);
        if (template == null)
        {
            throw new InvalidOperationException($"模板ID '{templateId}' 不存在");
        }

        return template.TemplateControls;
    }

    public async Task<Document> UpdateDocumentFromTemplateAsync(int documentId, Dictionary<string, object> fieldValues)
    {
        var document = await _documentService.GetDocumentAsync(documentId);
        if (document == null)
        {
            throw new InvalidOperationException($"收文ID '{documentId}' 不存在");
        }

        // 根据字段值更新文档内容
        if (fieldValues.ContainsKey("Title"))
        {
            document.Title = fieldValues["Title"]?.ToString() ?? document.Title;
        }
        
        if (fieldValues.ContainsKey("Sender"))
        {
            document.Sender = fieldValues["Sender"]?.ToString() ?? document.Sender;
        }
        
        if (fieldValues.ContainsKey("Handler"))
        {
            document.Handler = fieldValues["Handler"]?.ToString() ?? document.Handler;
        }

        // 更新状态为处理中
        if (document.Status == DocumentStatus.Received)
        {
            document.Status = DocumentStatus.Processing;
        }

        return await _documentService.UpdateDocumentAsync(document);
    }

    private string GenerateDocumentNumber()
    {
        // 生成格式：SW-YYYYMMDD-HHMMSS
        return $"SW-{DateTime.Now:yyyyMMdd-HHmmss}";
    }
}