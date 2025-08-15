using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DocumentManagement.Common;
using DocumentManagement.Core;
using DocumentManagement.Models;

namespace DocumentManagement.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDocumentService _documentService;
        private List<Document> _allDocuments;
        private List<Document> _filteredDocuments;
        private readonly DispatcherTimer _timeTimer;

        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化服务
            _documentService = new DocumentService();
            _allDocuments = new List<Document>();
            _filteredDocuments = new List<Document>();

            // 初始化计时器
            _timeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timeTimer.Tick += TimeTimer_Tick;
            _timeTimer.Start();

            // 加载数据
            LoadDataAsync();
        }

        /// <summary>
        /// 时间更新
        /// </summary>
        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            TimeText.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        private async void LoadDataAsync()
        {
            try
            {
                StatusText.Text = "正在加载数据...";
                
                // 加载文档列表
                var documents = await _documentService.GetAllDocumentsAsync();
                _allDocuments = documents.ToList();
                _filteredDocuments = new List<Document>(_allDocuments);
                
                // 更新UI
                UpdateDocumentsList();
                await UpdateStatisticsAsync();
                
                StatusText.Text = $"已加载 {_allDocuments.Count} 个文档";
            }
            catch (Exception ex)
            {
                StatusText.Text = "数据加载失败";
                MessageBox.Show($"加载数据时发生错误：{ex.Message}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 更新文档列表显示
        /// </summary>
        private void UpdateDocumentsList()
        {
            DocumentsDataGrid.ItemsSource = null;
            DocumentsDataGrid.ItemsSource = _filteredDocuments;
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        private async Task UpdateStatisticsAsync()
        {
            try
            {
                var statistics = await _documentService.GetDocumentStatisticsAsync();
                
                // 使用反射获取统计数据
                var statisticsType = statistics.GetType();
                
                var totalCount = statisticsType.GetProperty("TotalCount")?.GetValue(statistics) ?? 0;
                var pendingCount = statisticsType.GetProperty("PendingCount")?.GetValue(statistics) ?? 0;
                var todayCount = statisticsType.GetProperty("TodayReceivedCount")?.GetValue(statistics) ?? 0;
                var urgentCount = statisticsType.GetProperty("UrgentCount")?.GetValue(statistics) ?? 0;

                TotalCountText.Text = totalCount.ToString();
                PendingCountText.Text = pendingCount.ToString();
                TodayCountText.Text = todayCount.ToString();
                UrgentCountText.Text = urgentCount.ToString();
            }
            catch (Exception ex)
            {
                StatusText.Text = "统计信息更新失败";
                Console.WriteLine($"更新统计信息时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 新建文档按钮点击事件
        /// </summary>
        private void NewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newDocument = new Document
                {
                    Title = "新建文档",
                    DocumentNumber = Helpers.GenerateDocumentNumber("DOC"),
                    ReceivedDate = DateTime.Now,
                    Status = Constants.DocumentStatus.Draft,
                    UrgencyLevel = Constants.UrgencyLevel.Normal,
                    SecurityLevel = Constants.SecurityLevel.Internal
                };

                // 这里应该打开编辑窗口，暂时显示消息
                MessageBox.Show($"新建文档功能待实现\n文档编号：{newDocument.DocumentNumber}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"新建文档时发生错误：{ex.Message}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();
        }

        /// <summary>
        /// 搜索按钮点击事件
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Focus();
        }

        /// <summary>
        /// 搜索文本框按键事件
        /// </summary>
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchExecuteButton_Click(sender, e);
            }
        }

        /// <summary>
        /// 执行搜索按钮点击事件
        /// </summary>
        private async void SearchExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var keyword = SearchTextBox.Text.Trim();
                StatusText.Text = "正在搜索...";

                if (string.IsNullOrEmpty(keyword))
                {
                    // 如果搜索关键词为空，显示所有文档
                    _filteredDocuments = new List<Document>(_allDocuments);
                    ContentTitle.Text = "全部文档";
                }
                else
                {
                    // 执行搜索
                    var searchResults = await _documentService.SearchDocumentsAsync(keyword);
                    _filteredDocuments = searchResults.ToList();
                    ContentTitle.Text = $"搜索结果 - "{keyword}"";
                }

                UpdateDocumentsList();
                StatusText.Text = $"找到 {_filteredDocuments.Count} 个文档";
            }
            catch (Exception ex)
            {
                StatusText.Text = "搜索失败";
                MessageBox.Show($"搜索时发生错误：{ex.Message}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 筛选按钮点击事件
        /// </summary>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var filter = button?.Tag?.ToString();

                switch (filter)
                {
                    case "All":
                        _filteredDocuments = new List<Document>(_allDocuments);
                        ContentTitle.Text = "全部文档";
                        break;
                    case "Pending":
                        _filteredDocuments = _allDocuments.Where(d => d.Status == Constants.DocumentStatus.Pending).ToList();
                        ContentTitle.Text = "待办文档";
                        break;
                    case "Completed":
                        _filteredDocuments = _allDocuments.Where(d => d.Status == Constants.DocumentStatus.Completed).ToList();
                        ContentTitle.Text = "已完成文档";
                        break;
                    case "Urgent":
                        _filteredDocuments = _allDocuments.Where(d => 
                            d.UrgencyLevel == Constants.UrgencyLevel.Urgent || 
                            d.UrgencyLevel == Constants.UrgencyLevel.VeryUrgent ||
                            d.UrgencyLevel == Constants.UrgencyLevel.Emergency).ToList();
                        ContentTitle.Text = "紧急文档";
                        break;
                    default:
                        return;
                }

                UpdateDocumentsList();
                StatusText.Text = $"显示 {_filteredDocuments.Count} 个文档";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"筛选文档时发生错误：{ex.Message}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 文档列表选择改变事件
        /// </summary>
        private void DocumentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDocument = DocumentsDataGrid.SelectedItem as Document;
            var hasSelection = selectedDocument != null;

            // 更新按钮状态
            EditDocumentButton.IsEnabled = hasSelection;
            DeleteDocumentButton.IsEnabled = hasSelection;
            ViewDocumentButton.IsEnabled = hasSelection;

            if (hasSelection)
            {
                StatusText.Text = $"已选择文档：{selectedDocument.Title}";
            }
            else
            {
                StatusText.Text = "就绪";
            }
        }

        /// <summary>
        /// 编辑文档按钮点击事件
        /// </summary>
        private void EditDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDocument = DocumentsDataGrid.SelectedItem as Document;
            if (selectedDocument != null)
            {
                MessageBox.Show($"编辑文档功能待实现\n文档：{selectedDocument.Title}", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 删除文档按钮点击事件
        /// </summary>
        private async void DeleteDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDocument = DocumentsDataGrid.SelectedItem as Document;
            if (selectedDocument != null)
            {
                var result = MessageBox.Show($"确定要删除文档"{selectedDocument.Title}"吗？\n此操作不可撤销。", 
                    Constants.DefaultValues.SystemName, 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var success = await _documentService.DeleteDocumentAsync(selectedDocument.Id);
                        if (success)
                        {
                            MessageBox.Show("文档删除成功", Constants.DefaultValues.SystemName, 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataAsync(); // 重新加载数据
                        }
                        else
                        {
                            MessageBox.Show("文档删除失败", Constants.DefaultValues.SystemName, 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"删除文档时发生错误：{ex.Message}", 
                            Constants.DefaultValues.SystemName, 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 查看文档详情按钮点击事件
        /// </summary>
        private void ViewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDocument = DocumentsDataGrid.SelectedItem as Document;
            if (selectedDocument != null)
            {
                var details = $"文档详情：\n\n" +
                             $"标题：{selectedDocument.Title}\n" +
                             $"编号：{selectedDocument.DocumentNumber}\n" +
                             $"发文单位：{selectedDocument.SenderUnit}\n" +
                             $"收文日期：{selectedDocument.ReceivedDate:yyyy-MM-dd}\n" +
                             $"紧急程度：{selectedDocument.UrgencyLevel}\n" +
                             $"密级：{selectedDocument.SecurityLevel}\n" +
                             $"状态：{selectedDocument.Status}\n" +
                             $"创建时间：{selectedDocument.CreatedTime:yyyy-MM-dd HH:mm:ss}\n" +
                             $"更新时间：{selectedDocument.UpdatedTime:yyyy-MM-dd HH:mm:ss}\n\n" +
                             $"内容：\n{selectedDocument.Content}";

                MessageBox.Show(details, $"文档详情 - {selectedDocument.Title}", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 退出按钮点击事件
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要退出系统吗？", 
                Constants.DefaultValues.SystemName, 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 窗口关闭时停止计时器
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _timeTimer?.Stop();
            base.OnClosed(e);
        }
    }
}