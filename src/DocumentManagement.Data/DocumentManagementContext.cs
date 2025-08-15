using Microsoft.EntityFrameworkCore;
using DocumentManagement.Models;

namespace DocumentManagement.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class DocumentManagementContext : DbContext
{
    public DocumentManagementContext(DbContextOptions<DocumentManagementContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 收文表
    /// </summary>
    public DbSet<Document> Documents { get; set; }

    /// <summary>
    /// 文档模板表
    /// </summary>
    public DbSet<DocumentTemplate> DocumentTemplates { get; set; }

    /// <summary>
    /// 模板控件表
    /// </summary>
    public DbSet<TemplateControl> TemplateControls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置Document实体
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Sender).HasMaxLength(100);
            entity.Property(e => e.Handler).HasMaxLength(50);
            entity.HasIndex(e => e.DocumentNumber).IsUnique();
        });

        // 配置DocumentTemplate实体
        modelBuilder.Entity<DocumentTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        });

        // 配置TemplateControl实体
        modelBuilder.Entity<TemplateControl>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            
            // 配置外键关系
            entity.HasOne(e => e.Template)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // 默认使用SQLite数据库
            optionsBuilder.UseSqlite("Data Source=DocumentManagement.db");
        }
    }
}