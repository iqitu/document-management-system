using DocumentManagement.WPF.ViewModels;
using System.Windows;

namespace DocumentManagement.WPF.Views
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            // Since we can't use InitializeComponent in this environment,
            // we'll create the window programmatically
            CreateWindow();
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.InitializeAsync();
        }

        private void CreateWindow()
        {
            Title = "收文管理系统";
            Width = 1000;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}