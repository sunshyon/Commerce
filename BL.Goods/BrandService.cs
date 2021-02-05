using BL.Contracts;
using Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Goods
{
	public class BrandService: BaseService
	{
		private readonly OrangeContext _db;

		public BrandService(OrangeContext orangeContext)
		{
			_db = orangeContext;
		}
		public TbBrand QueryBrandByBid(long id)
		{
			TbBrand b1 = _db.TbBrand.Where(m => m.Id == id).FirstOrDefault();
			if (b1 == null)
			{
				throw new Exception("查询品牌不存在");
			}
			return b1;
		}
	}
}
