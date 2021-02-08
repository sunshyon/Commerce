using BL.Contracts;
using Domain.DbModels;
using System;
using System.Collections.Generic;

namespace BL.GoodsRelated
{
    public class PageDetailService : IPageDetailService
    {
        private readonly OrangeContext _db;

        public PageDetailService(OrangeContext db)
        {
            _db = db;
        }
        public Dictionary<string, object> LoadModel(long spuId)
        {
			Dictionary<string, object> model = new Dictionary<string, object>();
			TbSpu spu = new GoodsService(_db).QuerySpuBySpuId(spuId);

			//未上架，则不应该查询到商品详情信息，抛出异常
			if (spu.Saleable == null || spu.Saleable == false)
			{
				throw new Exception("查询了未上架的商品");
			}
			TbSpuDetail detail = spu.SpuDetail;
			List<TbSku> skus = spu.Skus;
			TbBrand brand = new BrandService(_db).QueryBrandByBid(spu.BrandId);
			//查询三级分类
			List<TbCategory> categories = new CategoryService(_db).QueryCategoryByIds(new List<long>() { spu.Cid1, spu.Cid2, spu.Cid3 });
			List<TbSpecGroup> specs = new SpecService(_db).QuerySpecsByCid(spu.Cid3);
			model.Add("brand", brand);
			model.Add("categories", categories);
			model.Add("spu", spu);
			model.Add("skus", skus);
			model.Add("detail", detail);
			model.Add("specs", specs);
			return model;
		}
    }
}
