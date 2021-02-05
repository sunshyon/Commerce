using Base.Common.Models;
using BL.Contracts;
using Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Goods
{
    public class GoodsService:BaseService
    {
        private readonly OrangeContext _db;

        public GoodsService(OrangeContext orangeContext)
        {
            _db = orangeContext;
        }

		public List<TbSku> QuerySkuBySpuId(long spuId)
		{
			TbSku sku = new TbSku();
			List<TbSku> skuList = _db.TbSku.Where(m => m.SpuId == spuId).ToList();
			if (skuList.Count <= 0)
			{
				throw new Exception("查询的商品的SKU失败");
			}
			//查询库存
			foreach (TbSku sku1 in skuList)
			{
				sku1.Stock = _db.TbStock.Where(m => m.SkuId == sku1.Id).FirstOrDefault().Stock;
			}
			return skuList;
		}

		public List<TbSku> QuerySkusByIds(List<long> ids)
		{
			List<TbSku> skus = _db.TbSku.Where(m => ids.Contains(m.Id)).ToList();
			if (skus.Count <= 0)
			{
				throw new Exception("查询");
			}
			//填充库存
			FillStock(ids, skus);
			return skus;
		}

		private void FillStock(List<long> ids, List<TbSku> skus)
		{
			//批量查询库存
			List<TbStock> stocks = _db.TbStock.Where(m => ids.Contains(m.SkuId)).ToList();
			if (stocks.Count <= 0)
			{
				throw new Exception("保存库存失败");
			}
			Dictionary<long, int> map = stocks.ToDictionary(s => s.SkuId, s => s.Stock);
			//首先将库存转换为map，key为sku的ID
			//遍历skus，并填充库存
			foreach (var sku in skus)
			{
				sku.Stock = map[sku.Id];
			}
		}


		public PageResult<TbSpu> QuerySpuByPage(int page, int rows, string key, bool? saleable)
		{
			var list = _db.TbSpu.AsQueryable();
			if (!string.IsNullOrEmpty(key))
			{
				list = list.Where(m => m.Title.Contains(key));
			}
			if (saleable != null)
			{
				list = list.Where(m => m.Saleable == saleable);
			}
			//默认以上一次更新时间排序
			list = list.OrderByDescending(m => m.LastUpdateTime);

			//只查询未删除的商品 
			list = list.Where(m => m.Valid == true);

			//查询
			List<TbSpu> spuList = list.ToList();

			if (spuList.Count <= 0)
			{
				throw new Exception("查询的商品不存在");
			}
			//对查询结果中的分类名和品牌名进行处理
			HandleCategoryAndBrand(spuList);
			return new PageResult<TbSpu>(spuList.Count, spuList);
		}

		/**
		 * 处理商品分类名和品牌名
		 *
		 * @param spuList
		 */
		private void HandleCategoryAndBrand(List<TbSpu> spuList)
		{
			foreach (TbSpu spu in spuList)
			{
				//根据spu中的分类ids查询分类名
				var ids = new List<string>() { spu.Cid1.ToString(), spu.Cid2.ToString(), spu.Cid3.ToString() };
				List<string> nameList = _db.TbCategory.Where(m => m.Id.ToString().Contains(m.Id.ToString())).Select(m => m.Name).ToList();
				//对分类名进行处理
				spu.Cname = string.Join('/', nameList);
				//查询品牌
				spu.Bname = _db.TbBrand.Where(m => m.Id == spu.BrandId).FirstOrDefault()?.Name;
			}
		}
		public TbSpu QuerySpuBySpuId(long spuId)
		{
			//根据spuId查询spu
			TbSpu spu = _db.TbSpu.Where(m => m.Id == spuId).FirstOrDefault();
			//查询spuDetail
			TbSpuDetail detail = QuerySpuDetailBySpuId(spuId);
			//查询skus
			List<TbSku> skus = QuerySkuBySpuId(spuId);
			spu.SpuDetail = detail;
			spu.Skus = skus;

			return spu;
		}

		public TbSpuDetail QuerySpuDetailBySpuId(long spuId)
		{
			TbSpuDetail spuDetail = _db.TbSpuDetail.Where(m => m.SpuId == spuId).FirstOrDefault();
			if (spuDetail == null)
			{
				throw new Exception("查询的商品不存在");
			}
			return spuDetail;
		}
	}
}
