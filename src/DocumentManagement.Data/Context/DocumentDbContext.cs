using DocumentManagement.Models;
using System.Data.Entity;

namespace DocumentManagement.Data.Context
{
    /// <summary>
    /// 文档管理系统数据库上下文
    /// </summary>
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext() : base("name=DocumentManagementConnection")
        {
            // 启用延迟加载
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DocumentDbContext(string connectionString) : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DbSet<DocumentRecord> DocumentRecords { get; set; }
        public DbSet<DocumentAttachment> DocumentAttachments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 配置 DocumentRecord 实体
            modelBuilder.Entity<DocumentRecord>()
                .HasKey(d => d.Id)
                .Property(d => d.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.SenderUnit)
                .HasMaxLength(100);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Category)
                .HasMaxLength(50);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Urgency)
                .HasMaxLength(20);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Handler)
                .HasMaxLength(50);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Department)
                .HasMaxLength(50);

            modelBuilder.Entity<DocumentRecord>()
                .Property(d => d.Remarks)
                .HasMaxLength(500);

            // 配置 DocumentAttachment 实体
            modelBuilder.Entity<DocumentAttachment>()
                .HasKey(a => a.Id)
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<DocumentAttachment>()
                .Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<DocumentAttachment>()
                .Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<DocumentAttachment>()
                .Property(a => a.ContentType)
                .HasMaxLength(50);

            // 配置外键关系
            modelBuilder.Entity<DocumentAttachment>()
                .HasRequired(a => a.DocumentRecord)
                .WithMany(d => d.Attachments)
                .HasForeignKey(a => a.DocumentRecordId)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}