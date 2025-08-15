using DocumentManagement.Services.Services;
using DocumentManagement.Core.Interfaces;
using DocumentManagement.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagement.Services.Tests
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDocumentRepository> _mockRepository;
        private DocumentService _documentService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IDocumentRepository>();
            _mockUnitOfWork.Setup(u => u.Documents).Returns(_mockRepository.Object);
            _documentService = new DocumentService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetDocumentByIdAsync_ValidId_ReturnsDocument()
        {
            // Arrange
            var documentId = 1;
            var expectedDocument = new DocumentRecord { Id = documentId, Title = "测试文档" };
            _mockRepository.Setup(r => r.GetByIdAsync(documentId))
                          .ReturnsAsync(expectedDocument);

            // Act
            var result = await _documentService.GetDocumentByIdAsync(documentId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(documentId);
            result.Title.Should().Be("测试文档");
        }

        [Test]
        public async Task CreateDocumentAsync_ValidDocument_ReturnsCreatedDocument()
        {
            // Arrange
            var document = new DocumentRecord 
            { 
                Title = "新文档",
                DocumentNumber = "202312001"
            };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<DocumentRecord>()))
                          .ReturnsAsync(document);

            // Act
            var result = await _documentService.CreateDocumentAsync(document);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("新文档");
            result.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task SearchDocumentsAsync_WithSearchTerm_ReturnsMatchingDocuments()
        {
            // Arrange
            var searchTerm = "测试";
            var documents = new List<DocumentRecord>
            {
                new DocumentRecord { Id = 1, Title = "测试文档1" },
                new DocumentRecord { Id = 2, Title = "测试文档2" },
                new DocumentRecord { Id = 3, Title = "其他文档" }
            };
            
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DocumentRecord, bool>>>()))
                          .ReturnsAsync(documents.Where(d => d.Title.Contains(searchTerm)));

            // Act
            var result = await _documentService.SearchDocumentsAsync(searchTerm);

            // Assert
            result.Should().HaveCount(2);
            result.All(d => d.Title.Contains(searchTerm)).Should().BeTrue();
        }

        [Test]
        public void CreateDocumentAsync_NullDocument_ThrowsArgumentNullException()
        {
            // Act & Assert
            Func<Task> act = async () => await _documentService.CreateDocumentAsync(null);
            act.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [TestFixture]
    public class DataLinkageServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDocumentRepository> _mockRepository;
        private DataLinkageService _dataLinkageService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IDocumentRepository>();
            _mockUnitOfWork.Setup(u => u.Documents).Returns(_mockRepository.Object);
            _dataLinkageService = new DataLinkageService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetNextDocumentNumberAsync_ReturnsCorrectFormat()
        {
            // Arrange
            var existingDocuments = new List<DocumentRecord>
            {
                new DocumentRecord { DocumentNumber = "2023120001" },
                new DocumentRecord { DocumentNumber = "2023120002" }
            };
            
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DocumentRecord, bool>>>()))
                          .ReturnsAsync(existingDocuments);

            // Act
            var result = await _dataLinkageService.GetNextDocumentNumberAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Length.Should().Be(10); // YYYYMM + 4位数字
            result.Should().StartWith(DateTime.Today.ToString("yyyyMM"));
        }

        [Test]
        public async Task GetSenderUnitsAsync_ReturnsDistinctUnits()
        {
            // Arrange
            var documents = new List<DocumentRecord>
            {
                new DocumentRecord { SenderUnit = "单位A" },
                new DocumentRecord { SenderUnit = "单位B" },
                new DocumentRecord { SenderUnit = "单位A" }, // 重复
                new DocumentRecord { SenderUnit = "" } // 空值
            };
            
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(documents);

            // Act
            var result = await _dataLinkageService.GetSenderUnitsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("单位A");
            result.Should().Contain("单位B");
        }
    }
}