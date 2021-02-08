using BL.Contracts;
using Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GoodsRelated
{
    public class CategoryService: BaseService
    {
        private readonly OrangeContext _db;

        public CategoryService(OrangeContext orangeContext)
        {
            _db = orangeContext;
        }
        public List<TbCategory> QueryCategoryByIds(List<long> ids)
        {
            return _db.TbCategory.Where(m => ids.Contains(m.Id)).ToList();
        }
    }
}
