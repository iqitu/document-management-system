using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentManagement.Models;
using DocumentManagement.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagement.WPF.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IDocumentService _documentService;
        private readonly IDataLinkageService _dataLinkageService;

        [ObservableProperty]
        private ObservableCollection<DocumentRecord> _documents;

        [ObservableProperty]
        private DocumentRecord _selectedDocument;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isLoading;

        public MainWindowViewModel(IDocumentService documentService, IDataLinkageService dataLinkageService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _dataLinkageService = dataLinkageService ?? throw new ArgumentNullException(nameof(dataLinkageService));
            
            Documents = new ObservableCollection<DocumentRecord>();
            
            // 初始化命令
            LoadDocumentsCommand = new AsyncRelayCommand(LoadDocumentsAsync);
            SearchCommand = new AsyncRelayCommand(SearchDocumentsAsync);
            AddDocumentCommand = new RelayCommand(AddDocument);
            EditDocumentCommand = new RelayCommand(EditDocument, CanEditDocument);
            DeleteDocumentCommand = new AsyncRelayCommand(DeleteDocumentAsync, CanDeleteDocument);
            RefreshCommand = new AsyncRelayCommand(LoadDocumentsAsync);
        }

        public ICommand LoadDocumentsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddDocumentCommand { get; }
        public ICommand EditDocumentCommand { get; }
        public ICommand DeleteDocumentCommand { get; }
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// 加载文档列表
        /// </summary>
        private async Task LoadDocumentsAsync()
        {
            try
            {
                IsLoading = true;
                var documents = await _documentService.GetAllDocumentsAsync();
                
                Documents.Clear();
                foreach (var doc in documents)
                {
                    Documents.Add(doc);
                }
            }
            catch (Exception ex)
            {
                // TODO: 添加日志记录和错误处理
                System.Windows.MessageBox.Show($"加载文档失败: {ex.Message}", "错误", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 搜索文档
        /// </summary>
        private async Task SearchDocumentsAsync()
        {
            try
            {
                IsLoading = true;
                var documents = string.IsNullOrWhiteSpace(SearchText) 
                    ? await _documentService.GetAllDocumentsAsync()
                    : await _documentService.SearchDocumentsAsync(SearchText);

                Documents.Clear();
                foreach (var doc in documents)
                {
                    Documents.Add(doc);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"搜索失败: {ex.Message}", "错误", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 添加新文档
        /// </summary>
        private void AddDocument()
        {
            // TODO: 打开添加文档对话框
            var newDocument = new DocumentRecord
            {
                DocumentNumber = "",
                Title = "",
                ReceiveDate = DateTime.Today,
                Status = DocumentStatus.Pending,
                CreatedDate = DateTime.Now
            };

            // 这里应该打开编辑窗口
            // 为了演示，暂时添加到列表中
            Documents.Insert(0, newDocument);
        }

        /// <summary>
        /// 编辑文档
        /// </summary>
        private void EditDocument()
        {
            if (SelectedDocument != null)
            {
                // TODO: 打开编辑文档对话框
                System.Windows.MessageBox.Show($"编辑文档: {SelectedDocument.Title}", "信息");
            }
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        private async Task DeleteDocumentAsync()
        {
            if (SelectedDocument == null) return;

            var result = System.Windows.MessageBox.Show(
                $"确定要删除文档 '{SelectedDocument.Title}' 吗?", 
                "确认删除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _documentService.DeleteDocumentAsync(SelectedDocument.Id);
                    Documents.Remove(SelectedDocument);
                    SelectedDocument = null;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"删除失败: {ex.Message}", "错误", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanEditDocument() => SelectedDocument != null;
        private bool CanDeleteDocument() => SelectedDocument != null;

        /// <summary>
        /// 初始化方法
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadDocumentsAsync();
        }
    }
}