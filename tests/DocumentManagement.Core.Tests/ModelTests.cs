using DocumentManagement.Models;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DocumentManagement.Core.Tests
{
    [TestFixture]
    public class DocumentRecordTests
    {
        [Test]
        public void DocumentRecord_Creation_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var document = new DocumentRecord();

            // Assert
            document.Id.Should().Be(0);
            document.Status.Should().Be(DocumentStatus.Pending);
            document.Attachments.Should().NotBeNull();
            document.Attachments.Should().BeEmpty();
        }

        [Test]
        public void DocumentRecord_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var receiveDate = DateTime.Today;
            var deadline = DateTime.Today.AddDays(7);

            // Act
            var document = new DocumentRecord
            {
                DocumentNumber = "202312001",
                Title = "测试文档",
                SenderUnit = "测试单位",
                ReceiveDate = receiveDate,
                Status = DocumentStatus.Processing,
                Category = "公文",
                Urgency = "普通",
                Handler = "张三",
                Department = "办公室",
                Deadline = deadline,
                Remarks = "测试备注"
            };

            // Assert
            document.DocumentNumber.Should().Be("202312001");
            document.Title.Should().Be("测试文档");
            document.SenderUnit.Should().Be("测试单位");
            document.ReceiveDate.Should().Be(receiveDate);
            document.Status.Should().Be(DocumentStatus.Processing);
            document.Category.Should().Be("公文");
            document.Urgency.Should().Be("普通");
            document.Handler.Should().Be("张三");
            document.Department.Should().Be("办公室");
            document.Deadline.Should().Be(deadline);
            document.Remarks.Should().Be("测试备注");
        }
    }

    [TestFixture]
    public class FormFieldTests
    {
        [Test]
        public void FormField_Creation_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var field = new FormField();

            // Assert
            field.FieldType.Should().Be(FormFieldType.TextBox);
            field.IsRequired.Should().BeFalse();
            field.MaxLength.Should().Be(0);
            field.Options.Should().NotBeNull();
            field.Options.Should().BeEmpty();
            field.Validation.Should().NotBeNull();
            field.Position.Should().NotBeNull();
            field.DependsOn.Should().NotBeNull();
            field.DependsOn.Should().BeEmpty();
        }

        [Test]
        public void FormField_WithConfiguration_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var field = new FormField
            {
                Name = "DocumentNumber",
                Label = "收文编号",
                FieldType = FormFieldType.TextBox,
                DefaultValue = "AUTO",
                IsRequired = true,
                MaxLength = 50,
                DataBinding = "DocumentNumber"
            };

            // Assert
            field.Name.Should().Be("DocumentNumber");
            field.Label.Should().Be("收文编号");
            field.FieldType.Should().Be(FormFieldType.TextBox);
            field.DefaultValue.Should().Be("AUTO");
            field.IsRequired.Should().BeTrue();
            field.MaxLength.Should().Be(50);
            field.DataBinding.Should().Be("DocumentNumber");
        }
    }
}