using Microsoft.EntityFrameworkCore;
using DocumentManagement.Models;

namespace DocumentManagement.Data;

/// <summary>
/// 数据库初始化器
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// 初始化数据库并添加示例数据
    /// </summary>
    public static async Task InitializeAsync(DocumentManagementContext context)
    {
        // 确保数据库创建
        await context.Database.EnsureCreatedAsync();

        // 如果已有数据，则不初始化
        if (await context.Documents.AnyAsync())
        {
            return;
        }

        // 添加示例文档模板
        var templates = new[]
        {
            new DocumentTemplate
            {
                Name = "收文登记表",
                Description = "标准收文登记表模板",
                FilePath = "/templates/receipt_form.docx",
                Metadata = """
                {
                    "ParsedAt": "2024-01-15T10:30:00",
                    "ControlCount": 8,
                    "FileExtension": ".docx",
                    "ControlTypes": {
                        "TextBox": 5,
                        "DatePicker": 2,
                        "ComboBox": 1
                    }
                }
                """,
                IsEnabled = true,
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now.AddDays(-30)
            },
            new DocumentTemplate
            {
                Name = "文件处理单",
                Description = "文件处理流程表单",
                FilePath = "/templates/process_form.docx",
                Metadata = """
                {
                    "ParsedAt": "2024-01-15T11:00:00",
                    "ControlCount": 6,
                    "FileExtension": ".docx",
                    "ControlTypes": {
                        "TextBox": 3,
                        "ComboBox": 2,
                        "CheckBox": 1
                    }
                }
                """,
                IsEnabled = true,
                CreatedAt = DateTime.Now.AddDays(-25),
                UpdatedAt = DateTime.Now.AddDays(-25)
            }
        };

        await context.DocumentTemplates.AddRangeAsync(templates);
        await context.SaveChangesAsync();

        // 为第一个模板添加控件
        var template1 = templates[0];
        var controls = new[]
        {
            new TemplateControl
            {
                TemplateId = template1.Id,
                Name = "DocumentNumber",
                Type = ControlType.TextBox,
                X = 10,
                Y = 10,
                Width = 200,
                Height = 25,
                Properties = """{"Label": "收文编号", "IsRequired": true}"""
            },
            new TemplateControl
            {
                TemplateId = template1.Id,
                Name = "Title",
                Type = ControlType.TextBox,
                X = 10,
                Y = 50,
                Width = 400,
                Height = 25,
                Properties = """{"Label": "文件标题", "IsRequired": true}"""
            },
            new TemplateControl
            {
                TemplateId = template1.Id,
                Name = "Sender",
                Type = ControlType.TextBox,
                X = 10,
                Y = 90,
                Width = 300,
                Height = 25,
                Properties = """{"Label": "发文单位", "IsRequired": true}"""
            },
            new TemplateControl
            {
                TemplateId = template1.Id,
                Name = "ReceivedDate",
                Type = ControlType.DatePicker,
                X = 10,
                Y = 130,
                Width = 150,
                Height = 25,
                Properties = """{"Label": "收文日期", "DefaultValue": "today"}"""
            },
            new TemplateControl
            {
                TemplateId = template1.Id,
                Name = "Status",
                Type = ControlType.ComboBox,
                X = 200,
                Y = 130,
                Width = 120,
                Height = 25,
                Properties = """{"Label": "状态", "Options": ["已接收", "处理中", "已完成", "已归档"]}"""
            }
        };

        await context.TemplateControls.AddRangeAsync(controls);
        await context.SaveChangesAsync();

        // 添加示例收文数据
        var documents = new[]
        {
            new Document
            {
                DocumentNumber = "SW-20240115-001",
                Title = "关于加强信息安全管理的通知",
                Sender = "市政府办公室",
                Content = "为进一步加强信息安全管理，现通知各部门...",
                ReceivedDate = DateTime.Now.AddDays(-10),
                Status = DocumentStatus.Completed,
                Handler = "张三",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-5)
            },
            new Document
            {
                DocumentNumber = "SW-20240118-002",
                Title = "2024年度工作计划报告",
                Sender = "人力资源部",
                Content = "根据公司发展战略，制定2024年度工作计划...",
                ReceivedDate = DateTime.Now.AddDays(-7),
                Status = DocumentStatus.Processing,
                Handler = "李四",
                CreatedAt = DateTime.Now.AddDays(-7),
                UpdatedAt = DateTime.Now.AddDays(-2)
            },
            new Document
            {
                DocumentNumber = "SW-20240120-003",
                Title = "设备采购申请",
                Sender = "技术部",
                Content = "因业务发展需要，申请采购以下设备...",
                ReceivedDate = DateTime.Now.AddDays(-3),
                Status = DocumentStatus.Received,
                Handler = "",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3)
            },
            new Document
            {
                DocumentNumber = "SW-20240122-004",
                Title = "员工培训计划",
                Sender = "培训中心",
                Content = "为提高员工技能水平，安排以下培训计划...",
                ReceivedDate = DateTime.Now.AddDays(-1),
                Status = DocumentStatus.Received,
                Handler = "",
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            }
        };

        await context.Documents.AddRangeAsync(documents);
        await context.SaveChangesAsync();
    }
}