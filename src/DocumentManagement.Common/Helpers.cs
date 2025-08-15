using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DocumentManagement.Common
{
    /// <summary>
    /// 辅助方法类
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// 生成唯一的文档编号
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns>唯一的文档编号</returns>
        public static string GenerateDocumentNumber(string prefix = "DOC")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}{timestamp}{random}";
        }

        /// <summary>
        /// 验证文件扩展名是否被允许
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="allowedExtensions">允许的扩展名数组</param>
        /// <returns>是否被允许</returns>
        public static bool IsAllowedFileExtension(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            foreach (var allowedExtension in allowedExtensions)
            {
                if (extension == allowedExtension.ToLowerInvariant())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 确保目录存在，如果不存在则创建
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        public static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 计算文件的MD5哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MD5哈希值</returns>
        public static string CalculateFileMD5(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// 获取安全的文件名，移除非法字符
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <returns>安全的文件名</returns>
        public static string GetSafeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "unnamed";

            var invalidChars = Path.GetInvalidFileNameChars();
            var safeFileName = fileName;

            foreach (var invalidChar in invalidChars)
            {
                safeFileName = safeFileName.Replace(invalidChar, '_');
            }

            return safeFileName;
        }

        /// <summary>
        /// 格式化文档编号显示
        /// </summary>
        /// <param name="documentNumber">文档编号</param>
        /// <returns>格式化的文档编号</returns>
        public static string FormatDocumentNumber(string documentNumber)
        {
            if (string.IsNullOrEmpty(documentNumber))
                return "无编号";

            // 如果编号包含日期部分，进行格式化
            if (documentNumber.Length >= 12)
            {
                var prefix = documentNumber.Substring(0, 3);
                var datePart = documentNumber.Substring(3, 8);
                var timePart = documentNumber.Substring(11, 6);
                var suffix = documentNumber.Substring(17);

                if (DateTime.TryParseExact(datePart, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    return $"{prefix}-{date:yyyy-MM-dd}-{timePart}-{suffix}";
                }
            }

            return documentNumber;
        }

        /// <summary>
        /// 验证邮箱地址格式
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>是否为有效邮箱格式</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成随机密码
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <param name="includeSpecialChars">是否包含特殊字符</param>
        /// <returns>随机密码</returns>
        public static string GenerateRandomPassword(int length = 8, bool includeSpecialChars = false)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var validChars = letters + numbers;
            if (includeSpecialChars)
                validChars += specialChars;

            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        /// <summary>
        /// 转换为中文数字
        /// </summary>
        /// <param name="number">阿拉伯数字</param>
        /// <returns>中文数字</returns>
        public static string ToChineseNumber(int number)
        {
            if (number == 0) return "零";

            string[] chineseNumbers = { "", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] units = { "", "十", "百", "千" };

            if (number < 0 || number > 9999)
                return number.ToString();

            string result = "";
            string numStr = number.ToString();
            int len = numStr.Length;

            for (int i = 0; i < len; i++)
            {
                int digit = int.Parse(numStr[i].ToString());
                int unitIndex = len - i - 1;

                if (digit != 0)
                {
                    result += chineseNumbers[digit] + units[unitIndex];
                }
                else if (result.Length > 0 && !result.EndsWith("零"))
                {
                    result += "零";
                }
            }

            return result.TrimEnd('零');
        }
    }
}