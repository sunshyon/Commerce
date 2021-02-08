using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Common.Models
{
    public class LogModel
    {
        /// <summary>
        /// 分布式日志
        /// </summary>
        public LogType Type { get; set; }
        public DateTime Time { get; set; }
        public string Url { get; set; }
        public string Parameter { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Msg { get; set; }
        public string StackTrace { get; set; }
    }

    public enum LogType
    {
        Info=1,
        Worning=2,
        Error=3
    }
}
