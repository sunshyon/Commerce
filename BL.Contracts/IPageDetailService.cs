using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IPageDetailService : IBaseService
    {
        public Dictionary<string, object> LoadModel(long spuId);
    }

}
