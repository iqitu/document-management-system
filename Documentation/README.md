# 收文管理系统 - 开发文档

## 项目概述

本项目是基于WPF技术的收文管理系统，主要功能包括：
- Word模板转WPF表单
- 收文信息管理
- 数据联动显示
- 文档状态跟踪

## 技术架构

### 项目结构
```
DocumentManagementSystem/
├── src/
│   ├── DocumentManagement.Models/        # 数据模型
│   ├── DocumentManagement.Core/          # 核心业务逻辑和接口
│   ├── DocumentManagement.Data/          # 数据访问层(Entity Framework)
│   ├── DocumentManagement.Services/      # 服务层
│   ├── DocumentManagement.WordParser/    # Word文档解析
│   └── DocumentManagement.WPF/           # WPF用户界面
├── tests/                                # 单元测试
├── Templates/                            # Word模板文件
├── Documentation/                        # 文档
└── Examples/                            # 示例文件
```

### 技术栈
- **目标框架**: .NET Framework 4.8 (兼容Windows 7)
- **UI框架**: WPF + MVVM 模式
- **MVVM框架**: CommunityToolkit.Mvvm
- **数据库**: SQLite + Entity Framework 6
- **Word处理**: DocumentFormat.OpenXml
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **日志**: NLog
- **测试**: NUnit + FluentAssertions + Moq

## 核心功能模块

### 1. Word模板解析 (DocumentManagement.WordParser)
- **WordTemplateParser**: 解析Word文档结构
- 支持表格、内容控件、文本框提取
- 自动生成WPF表单定义
- 样式映射和布局保持

### 2. 数据模型 (DocumentManagement.Models)
- **DocumentRecord**: 收文记录主实体
- **DocumentAttachment**: 附件信息
- **FormDefinition**: 动态表单定义
- **FormField**: 表单字段配置

### 3. 数据访问 (DocumentManagement.Data)
- **DocumentDbContext**: Entity Framework上下文
- **DocumentRepository**: 文档数据访问
- **UnitOfWork**: 工作单元模式

### 4. 服务层 (DocumentManagement.Services)
- **DocumentService**: 文档管理服务
- **DataLinkageService**: 数据联动服务
- **FormGenerationService**: 表单生成服务

### 5. WPF应用 (DocumentManagement.WPF)
- **MainWindow**: 主界面
- **MainWindowViewModel**: 主界面视图模型
- 依赖注入配置
- MVVM数据绑定

## 数据库设计

### DocumentRecords 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| Id | int | 主键，自增 |
| DocumentNumber | varchar(50) | 收文编号 |
| Title | varchar(200) | 标题 |
| SenderUnit | varchar(100) | 发文单位 |
| ReceiveDate | datetime | 收文日期 |
| Status | int | 处理状态(枚举) |
| Category | varchar(50) | 文件类别 |
| Urgency | varchar(20) | 紧急程度 |
| Handler | varchar(50) | 承办人 |
| Department | varchar(50) | 承办部门 |
| Deadline | datetime | 办理期限 |
| Remarks | varchar(500) | 备注 |
| CreatedDate | datetime | 创建时间 |
| ModifiedDate | datetime | 修改时间 |

### DocumentAttachments 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| Id | int | 主键，自增 |
| DocumentRecordId | int | 文档记录外键 |
| FileName | varchar(255) | 文件名 |
| FilePath | varchar(500) | 文件路径 |
| FileSize | bigint | 文件大小 |
| ContentType | varchar(50) | 文件类型 |
| UploadDate | datetime | 上传时间 |

## Word模板格式要求

### 支持的控件类型
1. **表格**: 标准Word表格，自动识别标签和输入区域
2. **内容控件**: 
   - 纯文本控件 → TextBox
   - 下拉列表控件 → ComboBox  
   - 日期选择器控件 → DatePicker
3. **文本框**: 文档中的文本框控件

### 字段映射规则
- 包含"编号"的字段 → DocumentNumber
- 包含"标题"的字段 → Title
- 包含"单位"的字段 → SenderUnit
- 包含"日期"的字段 → ReceiveDate (DatePicker)
- 包含"状态"的字段 → Status (ComboBox)
- 包含"备注"的字段 → Remarks (MultiLineText)

## 开发指南

### 环境要求
- Visual Studio 2019/2022
- .NET Framework 4.8 SDK
- Windows 7 SP1 或更高版本

### 构建步骤
1. 克隆代码库
2. 运行 `dotnet restore` 恢复NuGet包
3. 运行 `dotnet build` 编译项目
4. 运行 `dotnet test` 执行单元测试(需要Windows环境)

### 数据库初始化
首次运行时，系统会自动创建SQLite数据库文件 `DocumentManagement.db`

### 配置文件
编辑 `appsettings.json` 配置：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DocumentManagement.db;Version=3;"
  },
  "AppSettings": {
    "DefaultTemplatesPath": "Templates",
    "AttachmentsPath": "Attachments",
    "BackupPath": "Backups"
  }
}
```

## 扩展开发

### 添加新的表单字段类型
1. 在 `FormFieldType` 枚举中添加新类型
2. 在 `WordTemplateParser` 中添加识别逻辑
3. 在动态表单生成器中添加控件创建逻辑

### 添加新的数据联动规则
1. 在 `DataLinkageService` 中添加新的方法
2. 在视图模型中绑定联动逻辑
3. 在界面中设置数据绑定

### 自定义样式主题
1. 在 `App.xaml` 中定义样式资源
2. 支持动态主题切换
3. 响应系统主题变化

## 部署指南

### 单机部署
1. 编译Release版本
2. 复制输出文件到目标机器
3. 确保目标机器安装.NET Framework 4.8
4. 运行 `DocumentManagement.WPF.exe`

### 网络部署
1. 使用ClickOnce发布
2. 配置自动更新
3. 设置权限和证书

## 已知限制

1. Word模板解析依赖DocumentFormat.OpenXml，需要标准DOCX格式
2. 目标Windows 7系统需要安装.NET Framework 4.8
3. SQLite数据库适合中小型数据量，大数据量建议切换到SQL Server
4. WPF界面只支持Windows平台

## 后续开发计划

1. 添加更多Word控件类型支持
2. 实现表单验证规则引擎
3. 添加报表生成功能
4. 支持批量导入导出
5. 添加审批工作流
6. 实现全文搜索功能