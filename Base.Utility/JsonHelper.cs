using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utility
{
    public static class JsonHelper
    {
        public static string ToJson(this object obj)
        {
            if (obj == null || obj.ToString() == "")
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj);
        }
        public static T JsonToT<T>(this string jStr)
        {
            if (jStr == null || jStr == "")
            {
                return default(T);
            }
            if (typeof(T) == typeof(List<dynamic>))
            {
                var objRes = JsonConvert.DeserializeObject<List<dynamic>>(jStr);
                var r = Convert.ChangeType(objRes, typeof(T));
                return (T)r;
            }
            if (typeof(T) == typeof(object))//dynamic
            {
                var objRes = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jStr);
                return (T)objRes;
            }
            return JsonConvert.DeserializeObject<T>(jStr);
        }
        /// <summary>
        /// 骆驼峰式反序列化
        /// </summary>
        public static T JsonToTCamelCase<T>(this string jStr)
        {
            if (jStr == null || jStr == "")
            {
                return default(T);
            }

            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            var objRes = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jStr, settings);
            return (T)objRes;
        }

        public static string FromJsonFile(string fileName)
        {
            try
            {
                StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
                string json = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                return json;
            }
            catch
            {
                return null;
            }
        }

    }
}
