using NPOI.XWPF.UserModel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentManagement.Models;

namespace DocumentManagement.WordParser;

/// <summary>
/// Word文档解析器接口
/// </summary>
public interface IWordDocumentParser
{
    Task<DocumentTemplate> ParseWordTemplateAsync(string filePath, string templateName);
    Task<string> ExtractTextFromWordAsync(string filePath);
    Task<IEnumerable<TemplateControl>> ExtractControlsFromWordAsync(string filePath);
    bool IsWordDocument(string filePath);
}

/// <summary>
/// Word文档解析器实现
/// </summary>
public class WordDocumentParser : IWordDocumentParser
{
    public async Task<DocumentTemplate> ParseWordTemplateAsync(string filePath, string templateName)
    {
        if (!IsWordDocument(filePath))
        {
            throw new ArgumentException("文件不是有效的Word文档", nameof(filePath));
        }

        var template = new DocumentTemplate
        {
            Name = templateName,
            FilePath = filePath,
            IsEnabled = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // 解析控件
        var controls = await ExtractControlsFromWordAsync(filePath);
        foreach (var control in controls)
        {
            template.TemplateControls.Add(control);
        }

        // 生成元数据
        template.Metadata = GenerateMetadata(template);

        return template;
    }

    public async Task<string> ExtractTextFromWordAsync(string filePath)
    {
        try
        {
            if (Path.GetExtension(filePath).ToLower() == ".docx")
            {
                return await ExtractTextFromDocxAsync(filePath);
            }
            else if (Path.GetExtension(filePath).ToLower() == ".doc")
            {
                return await ExtractTextFromDocAsync(filePath);
            }
            else
            {
                throw new ArgumentException("不支持的文件格式", nameof(filePath));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解析Word文档失败: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<TemplateControl>> ExtractControlsFromWordAsync(string filePath)
    {
        var controls = new List<TemplateControl>();

        try
        {
            if (Path.GetExtension(filePath).ToLower() == ".docx")
            {
                controls.AddRange(await ExtractControlsFromDocxAsync(filePath));
            }
            else if (Path.GetExtension(filePath).ToLower() == ".doc")
            {
                controls.AddRange(await ExtractControlsFromDocAsync(filePath));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"提取控件失败: {ex.Message}", ex);
        }

        return controls;
    }

    public bool IsWordDocument(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return false;
        }

        var extension = Path.GetExtension(filePath).ToLower();
        return extension == ".doc" || extension == ".docx";
    }

    private async Task<string> ExtractTextFromDocxAsync(string filePath)
    {
        using var document = WordprocessingDocument.Open(filePath, false);
        var body = document.MainDocumentPart?.Document?.Body;
        return body?.InnerText ?? string.Empty;
    }

    private async Task<string> ExtractTextFromDocAsync(string filePath)
    {
        // For .doc files, we'll use a simplified approach since HWPF might not be available
        try
        {
            // Try using NPOI for .docx format even for .doc files
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var document = new XWPFDocument(fileStream);
            
            var text = "";
            foreach (var para in document.Paragraphs)
            {
                text += para.Text + "\n";
            }
            return text;
        }
        catch
        {
            // Fallback: read as binary and extract basic text
            var bytes = await File.ReadAllBytesAsync(filePath);
            var text = System.Text.Encoding.UTF8.GetString(bytes);
            return System.Text.RegularExpressions.Regex.Replace(text, @"[^\u0020-\u007E\u4e00-\u9fff]", "");
        }
    }

    private async Task<IEnumerable<TemplateControl>> ExtractControlsFromDocxAsync(string filePath)
    {
        var controls = new List<TemplateControl>();

        using var document = WordprocessingDocument.Open(filePath, false);
        var body = document.MainDocumentPart?.Document?.Body;
        
        if (body != null)
        {
            // 提取表格
            var tables = body.Elements<Table>();
            foreach (var table in tables)
            {
                var tableControl = new TemplateControl
                {
                    Name = $"Table_{controls.Count + 1}",
                    Type = ControlType.DataGrid,
                    X = 0,
                    Y = controls.Count * 50,
                    Width = 500,
                    Height = 200,
                    Properties = $"{{\"Rows\": {table.Elements<TableRow>().Count()}, \"Columns\": {table.Elements<TableRow>().FirstOrDefault()?.Elements<TableCell>().Count() ?? 0}}}"
                };
                controls.Add(tableControl);
            }

            // 提取文本框（简化实现，实际需要更复杂的逻辑）
            var paragraphs = body.Elements<Paragraph>();
            foreach (var para in paragraphs)
            {
                var text = para.InnerText;
                if (!string.IsNullOrWhiteSpace(text) && text.Contains("【") && text.Contains("】"))
                {
                    var textControl = new TemplateControl
                    {
                        Name = $"TextBox_{controls.Count + 1}",
                        Type = ControlType.TextBox,
                        X = 0,
                        Y = controls.Count * 30,
                        Width = 200,
                        Height = 25,
                        Properties = $"{{\"DefaultText\": \"{text}\"}}"
                    };
                    controls.Add(textControl);
                }
            }
        }

        return controls;
    }

    private async Task<IEnumerable<TemplateControl>> ExtractControlsFromDocAsync(string filePath)
    {
        var controls = new List<TemplateControl>();

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var document = new XWPFDocument(fileStream);
            
            // Extract tables
            foreach (var table in document.Tables)
            {
                var tableControl = new TemplateControl
                {
                    Name = $"Table_{controls.Count + 1}",
                    Type = ControlType.DataGrid,
                    X = 0,
                    Y = controls.Count * 50,
                    Width = 500,
                    Height = 200,
                    Properties = $"{{\"Rows\": {table.Rows.Count}, \"Columns\": {table.Rows.FirstOrDefault()?.GetTableCells().Count ?? 0}}}"
                };
                controls.Add(tableControl);
            }

            // Extract text fields
            foreach (var para in document.Paragraphs)
            {
                var text = para.Text;
                if (!string.IsNullOrWhiteSpace(text) && text.Contains("【") && text.Contains("】"))
                {
                    var textControl = new TemplateControl
                    {
                        Name = $"TextBox_{controls.Count + 1}",
                        Type = ControlType.TextBox,
                        X = 0,
                        Y = controls.Count * 30,
                        Width = 200,
                        Height = 25,
                        Properties = $"{{\"DefaultText\": \"{text}\"}}"
                    };
                    controls.Add(textControl);
                }
            }
        }
        catch
        {
            // Fallback: create a basic text control
            var control = new TemplateControl
            {
                Name = "BasicText_1",
                Type = ControlType.TextBox,
                X = 0,
                Y = 0,
                Width = 200,
                Height = 25,
                Properties = "{\"DefaultText\": \"Basic text control\"}"
            };
            controls.Add(control);
        }

        return controls;
    }

    private string GenerateMetadata(DocumentTemplate template)
    {
        var metadata = new
        {
            ParsedAt = DateTime.Now,
            ControlCount = template.TemplateControls.Count,
            FileExtension = Path.GetExtension(template.FilePath),
            ControlTypes = template.TemplateControls.GroupBy(c => c.Type)
                .ToDictionary(g => g.Key.ToString(), g => g.Count())
        };

        return System.Text.Json.JsonSerializer.Serialize(metadata);
    }
}