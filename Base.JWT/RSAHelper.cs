using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Base.JWT
{
    public class RSAHelper
    {
        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        public static bool TryGetKeyParameters(string filePath, bool withPrivate, out RSAParameters keyParameters)
        {
            var filename = withPrivate ? "key.json" : "key.public.json";
            var fileTotalPath = Path.Combine(filePath, filename);
            keyParameters = default(RSAParameters);
            if (!File.Exists(fileTotalPath))
                return false;
            else
            {
                keyParameters = JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(fileTotalPath));
                return true;
            }
        }
        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        public static RSAParameters GenerateAndSaveKey(string filePath, bool withPrivate = true)
        {
            RSAParameters publicKeys, privateKeys;
            using (var rsa = new RSACryptoServiceProvider(2048))//生成公钥私钥
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);//私钥--用于加密，保存在授权中心
                    publicKeys = rsa.ExportParameters(false);//公钥--用于解密，公开给用户
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            /*
             * 注意 JsonSerializer.Serialize 不行
             */
            //File.WriteAllText(Path.Combine(filePath, "key1.json"), JsonSerializer.Serialize<object>(privateKeys));
            File.WriteAllText(Path.Combine(filePath, "key.json"), JsonConvert.SerializeObject(privateKeys));
            File.WriteAllText(Path.Combine(filePath, "key.public.json"), JsonConvert.SerializeObject(publicKeys));
            return withPrivate ? privateKeys : publicKeys;
        }
    }
}
