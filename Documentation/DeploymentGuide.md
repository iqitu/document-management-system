# 收文管理系统 - 部署指南

## 系统要求

### 硬件要求
- **处理器**: Intel/AMD x86-64 双核2GHz或以上
- **内存**: 4GB RAM (推荐8GB或以上)
- **硬盘空间**: 200MB可用空间 (数据库和附件需要额外空间)
- **显示器**: 1024x768分辨率或以上

### 软件要求
- **操作系统**: Windows 7 SP1 / Windows 8.1 / Windows 10 / Windows 11
- **运行时环境**: .NET Framework 4.8
- **数据库**: SQLite (系统自带，无需单独安装)

## 部署前准备

### 1. 安装.NET Framework 4.8
如果目标系统未安装.NET Framework 4.8，请先安装：

1. 下载 Microsoft .NET Framework 4.8
2. 运行安装程序并按照向导完成安装
3. 重启计算机

### 2. 检查系统权限
- 确保用户具有文件读写权限
- 如需安装到Program Files目录，需要管理员权限

## 部署方式

### 方式一：直接复制部署 (推荐)

1. **编译发布版本**
   ```bash
   dotnet publish src/DocumentManagement.WPF/DocumentManagement.WPF.csproj -c Release -f net48
   ```

2. **创建部署目录**
   在目标计算机创建应用程序目录，例如：
   ```
   C:\Program Files\DocumentManagement\
   ```

3. **复制文件**
   将以下文件复制到部署目录：
   ```
   DocumentManagement.WPF.exe          # 主程序
   DocumentManagement.WPF.exe.config   # 配置文件
   appsettings.json                     # 应用设置
   *.dll                                # 依赖库文件
   Templates/                           # 模板目录(可选)
   ```

4. **创建数据目录**
   ```
   Attachments/    # 附件存储目录
   Backups/        # 备份目录
   Logs/           # 日志目录
   ```

5. **设置快捷方式**
   在桌面或开始菜单创建快捷方式指向 `DocumentManagement.WPF.exe`

### 方式二：MSI安装包部署

1. **创建安装项目**
   使用Visual Studio Installer Projects或WiX Toolset创建MSI安装包

2. **配置安装参数**
   - 安装目录：`%ProgramFiles%\DocumentManagement`
   - 数据目录：`%USERPROFILE%\Documents\DocumentManagement`
   - 注册表项：记录安装信息

3. **分发安装包**
   将MSI文件分发给用户进行安装

### 方式三：ClickOnce部署

1. **配置ClickOnce发布**
   在Visual Studio中配置ClickOnce发布设置

2. **设置更新策略**
   - 自动检查更新
   - 强制更新策略
   - 更新服务器地址

3. **发布到Web服务器**
   将发布文件上传到Web服务器供用户下载安装

## 配置文件设置

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DocumentManagement.db;Version=3;"
  },
  "AppSettings": {
    "DefaultTemplatesPath": "Templates",
    "AttachmentsPath": "Attachments",
    "BackupPath": "Backups",
    "BackupRetentionDays": 30,
    "AutoBackupEnabled": true,
    "Theme": "Light",
    "Language": "zh-CN"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### 数据库连接字符串配置
支持以下几种配置方式：

1. **默认SQLite数据库**
   ```json
   "DefaultConnection": "Data Source=DocumentManagement.db;Version=3;"
   ```

2. **指定数据库文件路径**
   ```json
   "DefaultConnection": "Data Source=C:\\Data\\DocumentManagement.db;Version=3;"
   ```

3. **网络共享数据库**
   ```json
   "DefaultConnection": "Data Source=\\\\Server\\Share\\DocumentManagement.db;Version=3;"
   ```

## 首次运行设置

### 1. 启动应用程序
双击 `DocumentManagement.WPF.exe` 启动程序

### 2. 数据库初始化
- 系统会自动创建SQLite数据库文件
- 初始化表结构
- 显示主界面

### 3. 配置检查
检查以下配置是否正确：
- 模板文件目录是否存在
- 附件存储目录是否可写
- 日志文件是否能正常创建

## 数据迁移

### 从旧版本升级
1. 备份现有数据库文件
2. 停止旧版本程序
3. 安装新版本程序
4. 启动程序，系统会自动执行数据库升级脚本

### 数据导入
支持以下格式的数据导入：
- Excel文件 (.xlsx)
- CSV文件 (.csv)
- 其他系统的数据库备份

## 多用户部署

### 单机多用户
- 每个用户使用独立的数据库文件
- 配置文件存储在用户目录下
- 模板和设置可以共享

### 网络部署
1. **共享数据库方式**
   - 将数据库文件放在网络共享目录
   - 所有客户端连接同一数据库
   - 需要处理并发访问问题

2. **数据库服务器方式**
   - 升级到SQL Server Express或完整版
   - 修改连接字符串指向数据库服务器
   - 支持更多并发用户

## 性能优化

### 1. 数据库优化
- 定期清理过期数据
- 重建数据库索引
- 压缩数据库文件

### 2. 文件管理
- 定期清理临时文件
- 移动旧附件到归档目录
- 压缩日志文件

### 3. 内存优化
- 关闭不必要的后台程序
- 增加系统内存
- 调整虚拟内存设置

## 备份与恢复

### 自动备份
系统默认启用自动备份功能：
- 每日自动备份数据库
- 保留最近30天的备份
- 备份文件存储在Backups目录

### 手动备份
1. 停止应用程序
2. 复制以下文件：
   - `DocumentManagement.db` (数据库文件)
   - `Attachments/` (附件目录)
   - `appsettings.json` (配置文件)
3. 保存到安全位置

### 数据恢复
1. 停止应用程序
2. 用备份文件替换现有文件
3. 重启应用程序
4. 验证数据完整性

## 常见问题

### Q: 程序无法启动
A: 
1. 检查.NET Framework 4.8是否已安装
2. 检查文件权限是否正确
3. 查看Windows事件日志中的错误信息

### Q: 数据库连接失败
A:
1. 检查数据库文件是否存在
2. 验证连接字符串配置
3. 确保数据库文件没有被其他程序占用

### Q: Word模板无法解析
A:
1. 确保模板文件是.docx格式
2. 检查模板文件是否损坏
3. 验证DocumentFormat.OpenXml组件是否正常

### Q: 性能较慢
A:
1. 检查硬盘空间是否充足
2. 清理过期数据和日志
3. 考虑升级硬件配置

## 技术支持

### 日志文件位置
- 应用程序日志：`Logs/app.log`
- 错误日志：`Logs/error.log`
- 调试日志：`Logs/debug.log`

### 联系支持
- 技术支持邮箱：support@company.com
- 用户文档：查看Documentation目录
- 在线帮助：系统内置帮助文档

### 系统监控
建议定期检查：
- 磁盘空间使用情况
- 数据库文件大小
- 备份文件的完整性
- 系统日志中的错误信息