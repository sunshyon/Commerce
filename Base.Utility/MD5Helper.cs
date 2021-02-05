using System;
using System.Security.Cryptography;
using System.Text;

namespace Base.Utility
{
	/// <summary>
	/// 任何长度都会转为32位固定长度
	/// </summary>
    public class MD5Helper
    {
		/// <summary>
		/// MD5 加密字符串
		/// </summary>
		/// <param name="content">源字符串</param>
		public static string MD5EncodingOnly(string content)
		{
			// 创建MD5类的默认实例：MD5CryptoServiceProvider
			MD5 md5 = MD5.Create();
			byte[] bs = Encoding.UTF8.GetBytes(content);
			byte[] hs = md5.ComputeHash(bs);
			StringBuilder stb = new StringBuilder();
			foreach (byte b in hs)
			{
				// 以十六进制格式格式化
				stb.Append(b.ToString("x2"));
			}
			return stb.ToString();
		}

		/// <summary>
		/// MD5盐值加密
		/// </summary>
		/// <param name="content">源字符串</param>
		/// <param name="salt">盐值</param>
		public static string MD5EncodingWithSalt(string content, string salt)
		{
			if (salt == null) return content;
			return MD5EncodingOnly(content + "{" + salt.ToString() + "}");
		}
	}
}
