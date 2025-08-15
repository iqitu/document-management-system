using System;
using System.Linq;
using System.Windows;
using DocumentManagement.Common;
using DocumentManagement.Core;
using DocumentManagement.Data;

namespace DocumentManagement.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 设置全局异常处理
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // 初始化数据库
                InitializeDatabase();

                // 显示主窗口
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序启动失败：{ex.Message}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                
                this.Shutdown();
            }
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                using (var context = new DocumentContext())
                {
                    // 确保数据库已创建
                    context.Database.CreateIfNotExists();
                    
                    // 检查是否需要初始化示例数据
                    if (!context.Users.Any())
                    {
                        InitializeSampleData(context);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"数据库初始化失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 初始化示例数据
        /// </summary>
        /// <param name="context">数据库上下文</param>
        private void InitializeSampleData(DocumentContext context)
        {
            try
            {
                // 创建示例部门
                var department = new DocumentManagement.Models.Department
                {
                    Name = "办公室",
                    Code = "BGS",
                    Description = "负责日常行政管理工作",
                    Level = 1,
                    SortOrder = 1,
                    IsEnabled = true
                };
                context.Departments.Add(department);

                // 创建示例用户
                var user = new DocumentManagement.Models.User
                {
                    Username = "admin",
                    FullName = "系统管理员",
                    Email = "admin@example.com",
                    Role = Constants.UserRole.Administrator,
                    IsEnabled = true,
                    Department = department
                };
                context.Users.Add(user);

                // 创建示例模板
                var template = new DocumentManagement.Models.Template
                {
                    Name = "通用收文模板",
                    Description = "适用于一般公文的收文模板",
                    TemplateType = "Word",
                    Version = "1.0",
                    IsEnabled = true,
                    CreatedUser = user,
                    SortOrder = 1,
                    Metadata = @"{
                        ""fields"": [
                            {""name"": ""Title"", ""displayName"": ""文档标题"", ""type"": ""TextBox"", ""required"": true},
                            {""name"": ""SenderUnit"", ""displayName"": ""发文单位"", ""type"": ""TextBox"", ""required"": true},
                            {""name"": ""ReceivedDate"", ""displayName"": ""收文日期"", ""type"": ""DatePicker"", ""required"": true},
                            {""name"": ""UrgencyLevel"", ""displayName"": ""紧急程度"", ""type"": ""ComboBox"", ""required"": false},
                            {""name"": ""Content"", ""displayName"": ""文档内容"", ""type"": ""RichTextBox"", ""required"": true}
                        ]
                    }"
                };
                context.Templates.Add(template);

                // 创建示例文档
                var document = new DocumentManagement.Models.Document
                {
                    Title = "关于召开年度工作会议的通知",
                    DocumentNumber = Helpers.GenerateDocumentNumber("DOC"),
                    SenderUnit = "办公室",
                    ReceivedDate = DateTime.Now.AddDays(-3),
                    UrgencyLevel = Constants.UrgencyLevel.Normal,
                    SecurityLevel = Constants.SecurityLevel.Internal,
                    Status = Constants.DocumentStatus.Pending,
                    Content = "根据年度工作安排，定于下月15日召开年度工作会议，请各部门做好准备。",
                    PrimaryDepartment = department,
                    ResponsibleUser = user,
                    CreatedUser = user,
                    Template = template
                };
                context.Documents.Add(document);

                // 保存更改
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"示例数据初始化失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"应用程序发生错误：{e.Exception.Message}", 
                Constants.DefaultValues.SystemName, 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            
            e.Handled = true;
        }

        /// <summary>
        /// 处理非UI线程异常
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            MessageBox.Show($"应用程序发生严重错误：{exception?.Message ?? "未知错误"}", 
                Constants.DefaultValues.SystemName, 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
    }
}