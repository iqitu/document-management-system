using System.Data.Entity;
using DocumentManagement.Models;
using DocumentManagement.Common;

namespace DocumentManagement.Data
{
    /// <summary>
    /// Entity Framework数据上下文
    /// </summary>
    public class DocumentContext : DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DocumentContext() : base("DocumentManagementConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DocumentContext>());
        }

        /// <summary>
        /// 文档数据集
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// 模板数据集
        /// </summary>
        public DbSet<Template> Templates { get; set; }

        /// <summary>
        /// 部门数据集
        /// </summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>
        /// 用户数据集
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 模型创建时的配置
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置Document实体
            modelBuilder.Entity<Document>()
                .HasKey(d => d.Id)
                .Property(d => d.Title).IsRequired().HasMaxLength(200);

            modelBuilder.Entity<Document>()
                .Property(d => d.DocumentNumber).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Document>()
                .Property(d => d.SenderUnit).HasMaxLength(100);

            modelBuilder.Entity<Document>()
                .Property(d => d.UrgencyLevel).HasMaxLength(20);

            modelBuilder.Entity<Document>()
                .Property(d => d.SecurityLevel).HasMaxLength(20);

            modelBuilder.Entity<Document>()
                .Property(d => d.Status).HasMaxLength(20);

            modelBuilder.Entity<Document>()
                .Property(d => d.AttachmentPath).HasMaxLength(500);

            modelBuilder.Entity<Document>()
                .Property(d => d.CopyDepartmentIds).HasMaxLength(500);

            // 配置外键关系
            modelBuilder.Entity<Document>()
                .HasOptional(d => d.PrimaryDepartment)
                .WithMany()
                .HasForeignKey(d => d.PrimaryDepartmentId);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.ResponsibleUser)
                .WithMany()
                .HasForeignKey(d => d.ResponsibleUserId);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.CreatedUser)
                .WithMany()
                .HasForeignKey(d => d.CreatedUserId);

            modelBuilder.Entity<Document>()
                .HasOptional(d => d.Template)
                .WithMany()
                .HasForeignKey(d => d.TemplateId);

            // 配置Template实体
            modelBuilder.Entity<Template>()
                .HasKey(t => t.Id)
                .Property(t => t.Name).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Template>()
                .Property(t => t.Description).HasMaxLength(500);

            modelBuilder.Entity<Template>()
                .Property(t => t.FilePath).IsRequired().HasMaxLength(500);

            modelBuilder.Entity<Template>()
                .Property(t => t.TemplateType).HasMaxLength(50);

            modelBuilder.Entity<Template>()
                .Property(t => t.Version).HasMaxLength(20);

            modelBuilder.Entity<Template>()
                .HasOptional(t => t.CreatedUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedUserId);

            // 配置Department实体
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Id)
                .Property(d => d.Name).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Department>()
                .Property(d => d.Code).HasMaxLength(20);

            modelBuilder.Entity<Department>()
                .Property(d => d.Description).HasMaxLength(500);

            modelBuilder.Entity<Department>()
                .Property(d => d.ContactPhone).HasMaxLength(50);

            modelBuilder.Entity<Department>()
                .Property(d => d.OfficeAddress).HasMaxLength(200);

            modelBuilder.Entity<Department>()
                .HasOptional(d => d.ParentDepartment)
                .WithMany()
                .HasForeignKey(d => d.ParentDepartmentId);

            modelBuilder.Entity<Department>()
                .HasOptional(d => d.ManagerUser)
                .WithMany()
                .HasForeignKey(d => d.ManagerUserId);

            // 配置User实体
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id)
                .Property(u => u.Username).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.FullName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber).HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(u => u.Position).HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Role).HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Remarks).HasMaxLength(500);

            modelBuilder.Entity<User>()
                .Property(u => u.EmployeeNumber).HasMaxLength(20);

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId);
        }

        /// <summary>
        /// 保存更改前的操作
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            // 在保存前自动更新时间戳
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Document document)
                {
                    if (entry.State == EntityState.Added)
                    {
                        document.CreatedTime = System.DateTime.Now;
                        document.UpdatedTime = System.DateTime.Now;
                        
                        if (string.IsNullOrEmpty(document.DocumentNumber))
                        {
                            document.DocumentNumber = Helpers.GenerateDocumentNumber("DOC");
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        document.UpdatedTime = System.DateTime.Now;
                    }
                }
                else if (entry.Entity is Template template)
                {
                    if (entry.State == EntityState.Added)
                    {
                        template.CreatedTime = System.DateTime.Now;
                        template.UpdatedTime = System.DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        template.UpdatedTime = System.DateTime.Now;
                    }
                }
                else if (entry.Entity is Department department)
                {
                    if (entry.State == EntityState.Added)
                    {
                        department.CreatedTime = System.DateTime.Now;
                        department.UpdatedTime = System.DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        department.UpdatedTime = System.DateTime.Now;
                    }
                }
                else if (entry.Entity is User user)
                {
                    if (entry.State == EntityState.Added)
                    {
                        user.CreatedTime = System.DateTime.Now;
                        user.UpdatedTime = System.DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        user.UpdatedTime = System.DateTime.Now;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}