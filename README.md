# 收文管理系统 (Document Management System)

一个基于WPF和Material Design的现代化收文管理系统，支持Word模板解析、文档管理和自动化工作流程。

## 技术架构

### 框架和平台
- **.NET Framework 4.5** - 兼容Windows 7及以上系统
- **WPF (Windows Presentation Foundation)** - 现代化桌面应用程序框架
- **Entity Framework 6.4.4** - 数据访问层ORM框架
- **SQLite** - 轻量级本地数据库

### 核心依赖库
- **Prism.Wpf 7.2.0.1422** - MVVM架构和依赖注入
- **MaterialDesignThemes 2.6.0** - Material Design风格UI组件
- **NPOI 2.5.6** - Word文档解析和处理
- **Newtonsoft.Json 13.0.3** - JSON序列化和反序列化

## 项目结构

```
DocumentManagement.sln
├── src/
│   ├── DocumentManagement.WPF/          # WPF主应用程序
│   │   ├── App.xaml                     # 应用程序入口
│   │   ├── MainWindow.xaml              # 主窗口界面
│   │   └── packages.config              # NuGet包配置
│   ├── DocumentManagement.Core/         # 核心业务逻辑
│   │   ├── IDocumentService.cs          # 文档服务接口
│   │   ├── DocumentService.cs           # 文档服务实现
│   │   └── TemplateParser.cs            # 模板解析器
│   ├── DocumentManagement.Models/       # 数据模型
│   │   ├── Document.cs                  # 收文文档模型
│   │   ├── Template.cs                  # 模板模型
│   │   ├── Department.cs                # 部门模型
│   │   └── User.cs                      # 用户模型
│   ├── DocumentManagement.Data/         # 数据访问层
│   │   ├── DocumentContext.cs           # EF数据上下文
│   │   ├── IRepository.cs               # 通用仓储接口
│   │   └── DocumentRepository.cs        # 文档仓储实现
│   ├── DocumentManagement.TemplateEngine/ # 模板引擎
│   │   ├── WordTemplateParser.cs        # Word模板解析器
│   │   ├── WpfControlGenerator.cs       # WPF控件生成器
│   │   └── TemplateMetadata.cs          # 模板元数据
│   └── DocumentManagement.Common/       # 公共工具类
│       ├── Constants.cs                 # 系统常量
│       ├── Extensions.cs                # 扩展方法
│       └── Helpers.cs                   # 辅助工具
```

## 核心功能

### 1. 文档管理
- ✅ 新建、编辑、删除收文文档
- ✅ 文档列表展示和分页
- ✅ 文档搜索和筛选
- ✅ 文档状态管理（草稿、待办、处理中、已完成、已归档）
- ✅ 紧急程度和密级管理

### 2. 模板系统
- ✅ Word模板解析（支持.docx格式）
- ✅ 动态表单生成
- ✅ 模板字段验证
- ✅ 自定义控件生成（文本框、下拉框、日期选择器等）

### 3. 用户界面
- ✅ Material Design现代化界面
- ✅ 响应式布局设计
- ✅ 实时统计仪表板
- ✅ 快速筛选和搜索功能
- ✅ 直观的操作工具栏

### 4. 数据管理
- ✅ SQLite本地数据库
- ✅ Entity Framework Code First
- ✅ 自动数据库初始化
- ✅ 示例数据生成

## 系统要求

### 开发环境
- Windows 7/8/10/11
- Visual Studio 2015 或更高版本
- .NET Framework 4.5 SDK
- SQL Server LocalDB（可选）

### 运行环境
- Windows 7 或更高版本
- .NET Framework 4.5 Runtime
- 至少 100MB 可用磁盘空间

## 安装和部署

### 1. 开发环境搭建

```bash
# 克隆仓库
git clone https://github.com/iqitu/document-management-system.git
cd document-management-system

# 在Visual Studio中打开解决方案
# 文件 -> 打开 -> 项目/解决方案 -> DocumentManagement.sln
```

### 2. NuGet包还原

在Visual Studio中：
1. 右键点击解决方案
2. 选择"还原NuGet包"
3. 等待包下载完成

或使用包管理器控制台：
```powershell
Update-Package -reinstall
```

### 3. 编译和运行

```bash
# 在Visual Studio中按F5运行
# 或使用MSBuild命令行编译
msbuild DocumentManagement.sln /p:Configuration=Release
```

### 4. 数据库初始化

首次运行时，系统会自动：
- 创建SQLite数据库文件
- 初始化数据表结构
- 生成示例数据（管理员用户、示例部门、示例文档）

## 使用指南

### 首次启动
1. 运行应用程序
2. 系统自动初始化数据库和示例数据
3. 默认管理员账户：用户名 `admin`

### 基本操作
1. **查看文档列表** - 主界面显示所有收文文档
2. **新建文档** - 点击工具栏"新建收文"按钮
3. **搜索文档** - 在左侧搜索框输入关键词
4. **筛选文档** - 使用左侧快速筛选按钮
5. **查看详情** - 选中文档后点击"查看详情"按钮

### 高级功能
- **模板管理** - 导入Word模板，自动解析字段
- **状态跟踪** - 跟踪文档处理进度
- **统计报表** - 查看实时统计数据

## 配置说明

### 数据库配置
数据库连接字符串在 `DocumentManagement.Data/App.config` 中配置：

```xml
<connectionStrings>
  <add name="DocumentManagementConnection" 
       connectionString="Data Source=DocumentManagement.db;Version=3;" 
       providerName="System.Data.SQLite.EF6" />
</connectionStrings>
```

### 应用程序设置
系统配置常量在 `DocumentManagement.Common/Constants.cs` 中定义。

## 故障排除

### 常见问题

1. **数据库连接失败**
   - 检查SQLite数据库文件权限
   - 确保应用程序有写入权限

2. **模板解析失败**
   - 确保Word文档为.docx格式
   - 检查模板中的字段格式（使用 `{字段名}` 格式）

3. **界面显示异常**
   - 检查.NET Framework版本
   - 确保MaterialDesignThemes包正确安装

### 日志和调试
- 应用程序错误会显示在状态栏
- 详细错误信息通过MessageBox显示
- 可以在Visual Studio中设置断点进行调试

## 扩展开发

### 添加新功能
1. 在相应的项目中添加新类
2. 实现接口或继承基类
3. 在UI中添加相应的控件和事件处理

### 自定义模板字段
1. 在 `TemplateField` 类中添加新的字段类型
2. 在 `WpfControlGenerator` 中实现对应的控件生成逻辑
3. 在 `WordTemplateParser` 中添加解析规则

## 技术支持

如有问题或建议，请通过以下方式联系：
- GitHub Issues: [创建Issue](https://github.com/iqitu/document-management-system/issues)
- 项目Wiki: [查看文档](https://github.com/iqitu/document-management-system/wiki)

## 版本历史

### v1.0.0 (当前版本)
- ✅ 完整的WPF应用程序框架
- ✅ Material Design界面设计
- ✅ 基础文档管理功能
- ✅ Word模板解析功能
- ✅ SQLite数据库集成
- ✅ 实时统计和搜索功能

## 许可证

本项目遵循 MIT 许可证。详见 [LICENSE](LICENSE) 文件。