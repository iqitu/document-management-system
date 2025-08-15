namespace DocumentManagement.Common
{
    /// <summary>
    /// 系统常量定义
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 文档状态常量
        /// </summary>
        public static class DocumentStatus
        {
            public const string Draft = "草稿";
            public const string Pending = "待办";
            public const string InProgress = "处理中";
            public const string Completed = "已完成";
            public const string Archived = "已归档";
            public const string Cancelled = "已取消";
        }

        /// <summary>
        /// 紧急程度常量
        /// </summary>
        public static class UrgencyLevel
        {
            public const string Normal = "一般";
            public const string Urgent = "紧急";
            public const string VeryUrgent = "特急";
            public const string Emergency = "特提";
        }

        /// <summary>
        /// 密级常量
        /// </summary>
        public static class SecurityLevel
        {
            public const string Public = "公开";
            public const string Internal = "内部";
            public const string Confidential = "机密";
            public const string Secret = "秘密";
            public const string TopSecret = "绝密";
        }

        /// <summary>
        /// 用户角色常量
        /// </summary>
        public static class UserRole
        {
            public const string Administrator = "管理员";
            public const string Manager = "经理";
            public const string Clerk = "办事员";
            public const string Viewer = "查看者";
        }

        /// <summary>
        /// 文件路径常量
        /// </summary>
        public static class FilePaths
        {
            public const string TemplatesFolder = "Templates";
            public const string AttachmentsFolder = "Attachments";
            public const string TempFolder = "Temp";
            public const string DatabaseFile = "DocumentManagement.db";
        }

        /// <summary>
        /// 配置键常量
        /// </summary>
        public static class ConfigKeys
        {
            public const string DatabaseConnectionString = "ConnectionString";
            public const string DefaultDepartment = "DefaultDepartment";
            public const string SystemName = "SystemName";
            public const string Version = "Version";
        }

        /// <summary>
        /// 默认值常量
        /// </summary>
        public static class DefaultValues
        {
            public const string SystemName = "收文管理系统";
            public const string Version = "1.0.0";
            public const int DefaultPageSize = 20;
            public const int MaxFileSize = 10485760; // 10MB
        }
    }
}