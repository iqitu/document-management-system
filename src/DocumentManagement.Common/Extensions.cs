using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentManagement.Common
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 判断字符串是否为空或null
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果为空或null返回true，否则返回false</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 判断字符串是否为空、null或仅包含空白字符
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果为空、null或仅包含空白字符返回true，否则返回false</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 安全的Trim操作，不会抛出null异常
        /// </summary>
        /// <param name="value">要处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string SafeTrim(this string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// 将字符串截断到指定长度
        /// </summary>
        /// <param name="value">要截断的字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="appendEllipsis">是否添加省略号</param>
        /// <returns>截断后的字符串</returns>
        public static string Truncate(this string value, int maxLength, bool appendEllipsis = false)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            var truncated = value.Substring(0, maxLength);
            return appendEllipsis ? truncated + "..." : truncated;
        }

        /// <summary>
        /// 将集合转换为逗号分隔的字符串
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns>分隔的字符串</returns>
        public static string ToDelimitedString<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (source == null)
                return string.Empty;

            return string.Join(separator, source.Select(x => x?.ToString() ?? string.Empty));
        }

        /// <summary>
        /// 将逗号分隔的字符串转换为整数集合
        /// </summary>
        /// <param name="value">逗号分隔的字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns>整数集合</returns>
        public static IEnumerable<int> ToIntList(this string value, string separator = ",")
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<int>();

            return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => x.Trim())
                       .Where(x => int.TryParse(x, out _))
                       .Select(int.Parse);
        }

        /// <summary>
        /// 格式化文件大小显示
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化的文件大小字符串</returns>
        public static string ToFileSizeString(this long bytes)
        {
            const int scale = 1024;
            string[] orders = { "B", "KB", "MB", "GB", "TB" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                max /= scale;
            }
            return "0 B";
        }

        /// <summary>
        /// 获取友好的时间差描述
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns>友好的时间差描述</returns>
        public static string ToFriendlyTimeString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "刚刚";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}分钟前";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}小时前";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}天前";
            
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}