using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DocumentManagement.Core.Services;
using DocumentManagement.Data;
using DocumentManagement.Data.Repositories;
using DocumentManagement.Models;
using DocumentManagement.Services;
using DocumentManagement.WordParser;

namespace DocumentManagement.WPF;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("文档管理系统演示");
        Console.WriteLine("===============");
        Console.WriteLine();

        // 配置依赖注入
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // 初始化数据库
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DocumentManagementContext>();
            await DatabaseInitializer.InitializeAsync(context);
            Console.WriteLine("✅ 数据库初始化完成");

            // 演示核心功能
            await DemonstrateDocumentManagement(serviceProvider);
            await DemonstrateTemplateManagement(serviceProvider);
            await DemonstrateWordParsing(serviceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 程序执行出错: {ex.Message}");
        }
        finally
        {
            serviceProvider.Dispose();
        }

        Console.WriteLine();
        Console.WriteLine("演示完成，按任意键退出...");
        Console.ReadKey();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // 配置Entity Framework
        services.AddDbContext<DocumentManagementContext>(options =>
            options.UseSqlite("Data Source=DocumentManagement.db"));

        // 配置仓储
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();
        services.AddScoped<ITemplateControlRepository, TemplateControlRepository>();

        // 配置服务
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IWordDocumentParser, WordDocumentParser>();
        services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole());
    }

    private static async Task DemonstrateDocumentManagement(ServiceProvider serviceProvider)
    {
        Console.WriteLine("📄 文档管理功能演示");
        Console.WriteLine("=================");

        using var scope = serviceProvider.CreateScope();
        var documentService = scope.ServiceProvider.GetRequiredService<IDocumentService>();

        // 获取所有文档
        var allDocuments = await documentService.GetAllDocumentsAsync();
        Console.WriteLine($"📊 系统中共有 {allDocuments.Count()} 个文档");

        // 按状态分组显示
        var groupedByStatus = allDocuments.GroupBy(d => d.Status);
        foreach (var group in groupedByStatus)
        {
            Console.WriteLine($"   {GetStatusDisplayName(group.Key)}: {group.Count()} 个");
        }

        // 显示最新的3个文档
        Console.WriteLine();
        Console.WriteLine("📋 最新的3个文档:");
        var recentDocuments = allDocuments
            .OrderByDescending(d => d.ReceivedDate)
            .Take(3);

        foreach (var doc in recentDocuments)
        {
            Console.WriteLine($"   • {doc.DocumentNumber} - {doc.Title}");
            Console.WriteLine($"     发文单位: {doc.Sender} | 状态: {GetStatusDisplayName(doc.Status)}");
        }

        // 搜索功能演示
        Console.WriteLine();
        Console.WriteLine("🔍 搜索功能演示 (搜索关键字: '通知'):");
        var searchResults = await documentService.SearchDocumentsAsync("通知");
        foreach (var doc in searchResults)
        {
            Console.WriteLine($"   • {doc.DocumentNumber} - {doc.Title}");
        }

        Console.WriteLine();
    }

    private static async Task DemonstrateTemplateManagement(ServiceProvider serviceProvider)
    {
        Console.WriteLine("📝 模板管理功能演示");
        Console.WriteLine("=================");

        using var scope = serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();

        // 获取所有启用的模板
        var templates = await templateService.GetEnabledTemplatesAsync();
        Console.WriteLine($"📊 系统中共有 {templates.Count()} 个启用的模板");

        foreach (var template in templates)
        {
            Console.WriteLine($"   • {template.Name} - {template.Description}");
            
            // 获取模板控件
            var controls = await templateService.GetTemplateControlsAsync(template.Id);
            Console.WriteLine($"     包含 {controls.Count()} 个控件:");
            
            foreach (var control in controls.Take(3)) // 只显示前3个
            {
                Console.WriteLine($"       - {control.Name} ({GetControlTypeDisplayName(control.Type)})");
            }
            
            if (controls.Count() > 3)
            {
                Console.WriteLine($"       ... 还有 {controls.Count() - 3} 个控件");
            }
        }

        Console.WriteLine();
    }

    private static async Task DemonstrateWordParsing(ServiceProvider serviceProvider)
    {
        Console.WriteLine("📄 Word解析功能演示");
        Console.WriteLine("==================");

        using var scope = serviceProvider.CreateScope();
        var wordParser = scope.ServiceProvider.GetRequiredService<IWordDocumentParser>();

        // 创建一个示例Word文件
        var sampleWordFile = "/tmp/sample.docx";
        await CreateSampleWordFile(sampleWordFile);

        if (File.Exists(sampleWordFile))
        {
            Console.WriteLine($"✅ 创建示例Word文件: {sampleWordFile}");
            
            Console.WriteLine($"📝 Word文档类型检查: {(wordParser.IsWordDocument(sampleWordFile) ? "是Word文档" : "不是Word文档")}");
            
            try
            {
                // 提取文本
                var text = await wordParser.ExtractTextFromWordAsync(sampleWordFile);
                Console.WriteLine($"📄 提取的文本 (前100字符): {text.Substring(0, Math.Min(100, text.Length))}...");
                
                // 提取控件
                var controls = await wordParser.ExtractControlsFromWordAsync(sampleWordFile);
                Console.WriteLine($"🎛️ 解析出的控件数量: {controls.Count()}");
                
                foreach (var control in controls.Take(3))
                {
                    Console.WriteLine($"   • {control.Name} ({GetControlTypeDisplayName(control.Type)}) - 位置: ({control.X}, {control.Y})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Word解析过程中出现警告: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("⚠️ 无法创建示例Word文件，跳过Word解析演示");
        }

        Console.WriteLine();
    }

    private static async Task CreateSampleWordFile(string filePath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
            // 创建一个简单的文本文件作为示例（模拟Word文档）
            var sampleContent = """
                收文登记表

                收文编号：【SW-2024-001】
                文件标题：【关于系统升级的通知】
                发文单位：【技术部】
                收文日期：【2024年1月23日】
                处理状态：【待处理】
                
                文件内容：
                根据公司发展需要，现对文档管理系统进行升级...
                """;
            
            await File.WriteAllTextAsync(filePath, sampleContent);
        }
        catch
        {
            // 如果创建失败，不影响程序继续运行
        }
    }

    private static string GetStatusDisplayName(DocumentStatus status)
    {
        return status switch
        {
            DocumentStatus.Received => "已接收",
            DocumentStatus.Processing => "处理中",
            DocumentStatus.Completed => "已完成",
            DocumentStatus.Archived => "已归档",
            _ => status.ToString()
        };
    }

    private static string GetControlTypeDisplayName(ControlType type)
    {
        return type switch
        {
            ControlType.TextBox => "文本框",
            ControlType.ComboBox => "下拉列表",
            ControlType.DatePicker => "日期选择器",
            ControlType.CheckBox => "复选框",
            ControlType.RadioButton => "单选按钮",
            ControlType.Label => "标签",
            ControlType.DataGrid => "表格",
            _ => type.ToString()
        };
    }
}