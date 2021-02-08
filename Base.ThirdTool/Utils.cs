using System;
using System.Text.Json;

namespace Base.ThirdTool
{
    internal static class Utils
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
        public static T ToObj<T>(this object json)
        {
            var data = json.ToString();
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
