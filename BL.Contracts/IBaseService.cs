using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    /*
    动态WebApi说明： 必须.Net5以上
    1、IService和Startup须引入包Panda.DynamicWebApi
    2、IService中无需编码
    3、Servcie须被Startup引用
    4、IService中必须继承IDynamicWebApi
   */
    [DynamicWebApi]
    public interface IBaseService : IDynamicWebApi
    {
    }
}
