using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DocumentManagement.Data.Context;
using DocumentManagement.Data.Repositories;
using DocumentManagement.Core.Interfaces;
using DocumentManagement.Services.Interfaces;
using DocumentManagement.Services.Services;
using DocumentManagement.WordParser.Services;
using DocumentManagement.WordParser.FormGenerator;
using DocumentManagement.WPF.Views;
using DocumentManagement.WPF.ViewModels;

namespace DocumentManagement.WPF
{
    /// <summary>
    /// WPF应用程序入口
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 配置依赖注入
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // 启动主窗口
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 配置
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // 数据层
            services.AddScoped<DocumentDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=DocumentManagement.db";
                return new DocumentDbContext(connectionString);
            });

            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 服务层
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDataLinkageService, DataLinkageService>();
            services.AddScoped<IFormGenerationService, FormGenerationService>();

            // Word处理
            services.AddScoped<IWordTemplateParser, WordTemplateParser>();
            services.AddScoped<IDynamicFormGenerator, DynamicFormGenerator>();

            // 视图和视图模型
            services.AddScoped<MainWindow>();
            services.AddScoped<MainWindowViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}