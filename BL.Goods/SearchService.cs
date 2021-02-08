using BL.Contracts;
using Domain.DbModels;
using Domain.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Base.Utility;
using Base.ThirdTool;

namespace BL.GoodsRelated
{
    public class SearchService: BaseService
    {
        private readonly OrangeContext _orangeContext;
        private readonly GoodsService _goodsService;
        private readonly SpecService _specService;
        private readonly EsClient _elasticSearch;

        public SearchService(OrangeContext orangeContext,
            GoodsService goodsService,
            SpecService specService,
            EsClient elasticSearch
            )
        {
            _orangeContext = orangeContext;
            _goodsService = goodsService;
            _specService = specService;
            _elasticSearch = elasticSearch;
        }
        /// <summary>
        /// ES搜索查询
        /// </summary>
        public void EsQuery(string queryContent)
        {
             var client= _elasticSearch.GetEsClient();
            var list = client.Search<Goods>(s => s
               .Query(q => q
                    .Match(m => m
                       .Field(f => f.all)
                       .Query(queryContent)
                    )
               )).Documents.ToList();

            var total = client.Search<Goods>(s => s
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.all)
                        .Query(queryContent)
                     )
                )).Documents.Count();

            var cid3s = list.Select(m => m.cid3).Distinct().ToList();
            var brandIds = list.Select(m => m.brandId).Distinct().ToList();

            /*
              将结果进行业务聚合
              ....
            */
        }

        public Goods GetGoodsBySpuId(long spuId)
        {
            var spu = this._goodsService.QuerySpuBySpuId(spuId);
            return this.BuildGoods(spu);
        }

        #region 私有
        /// <summary>
        /// 根据Spu构建Goods对象
        /// </summary>
        /// <param name="spu"></param>
        private Goods BuildGoods(TbSpu spu)
        {
            Goods goods = new Goods();
            // 1、查询商品分类名称组成的集合
            //List<TbCategory> lists = _categoryService.QueryCategoryByIds(new List<long>() { spu.Cid1, spu.Cid2, spu.Cid3 });
            var ids = new List<long>() { spu.Cid1, spu.Cid2, spu.Cid3 };
            var lists= _orangeContext.TbCategory.Where(m => ids.Contains(m.Id)).ToList();
            var cnames = lists.Select(c => c.Name).ToList();
            // 2、根据品牌ID查询品牌信息
            //var brand = _brandService.QueryBrandByBid(spu.BrandId);
            var brand= _orangeContext.TbBrand.Where(m => m.Id == spu.BrandId).FirstOrDefault();
            // 3、所有的搜索字段拼接到all中，all存入索引库，并进行分词处理，搜索时与all中的字段进行匹配查询
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            string subTitle = regex.Replace(spu.SubTitle, "");
            string all = subTitle + " " + string.Join(" ", cnames) + " " + brand.Name;
            // 4、根据spu查询所有的sku集合
            List<TbSku> skuList = _goodsService.QuerySkuBySpuId(spu.Id);
            if (skuList == null && skuList.Count > 0)
            {
                throw new Exception("查询商品对应sku不存在");
            }
            //4.1 存储price的集合
            HashSet<double> priceSet = new HashSet<double>();
            //4.2 设置存储skus的json结构的集合，用map结果转化sku对象，转化为json之后与对象结构相似（或者重新定义一个对象，存储前台要展示的数据，并把sku对象转化成自己定义的对象）
            List<Dictionary<string, object>> skus = new List<Dictionary<string, object>>();
            foreach (TbSku sku in skuList)
            {
                priceSet.Add(sku.Price);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", sku.Id);
                dic.Add("title", sku.Title);
                //sku中有多个图片，只展示第一张
                dic.Add("image", sku.Images.Split(",")[0]);
                dic.Add("price", sku.Price);
                // 添加到字典中
                skus.Add(dic);
            }

            //查询规格参数，规格参数中分为通用规格参数和特有规格参数
            List<TbSpecParam> specParams = _specService.QuerySpecParams(null, spu.Cid3, true, null);
            if (specParams == null && specParams.Count > 0)
            {
                throw new Exception("规格参数不存在");
            }

            //查询商品详情
            TbSpuDetail spuDetail = _goodsService.QuerySpuDetailBySpuId(spu.Id);
            //获取通用规格参数
            Dictionary<long, string> genericSpec = JsonHelper.JsonToT<Dictionary<long, string>>(spuDetail.GenericSpec);
            //获取特有规格参数
            Dictionary<long, List<string>> specialSpec = JsonHelper.JsonToT<Dictionary<long, List<string>>>(spuDetail.SpecialSpec);
            //定义spec对应的map
            Dictionary<string, object> specDic = new Dictionary<string, object>();
            //对规格进行遍历，并封装spec，其中spec的key是规格参数的名称，值是商品详情中的值
            foreach (TbSpecParam param in specParams)
            {
                //key是规格参数的名称
                string key = param.Name;
                object value = "";

                if (param.Generic == true)
                {
                    //参数是通用属性，通过规格参数的ID从商品详情存储的规格参数中查出值
                    value = genericSpec[param.Id];
                    if (param.Numeric == true)
                    {
                        //参数是数值类型，处理成段，方便后期对数值类型进行范围过滤
                        value = ChooseSegment(value.ToString(), param);
                    }
                }
                else
                {
                    //参数不是通用类型
                    value = specialSpec[param.Id];
                }
                value ??= "其他";
                //存入map
                specDic.Add(key, value);
            }

            // 封装商品对象
            goods.id = spu.Id;
            goods.brandId = spu.BrandId;
            goods.cid1 = spu.Cid1;
            goods.cid2 = spu.Cid2;
            goods.cid3 = spu.Cid3;
            goods.createTime = spu.CreateTime;
            goods.all = all;
            goods.price = priceSet;
            goods.subtitle = spu.SubTitle;
            goods.specs = specDic;
            goods.skus = skus.ToJson();

            return goods;
        }
        private static string ChooseSegment(string value, TbSpecParam p)
        {
            try
            {
                double val = double.Parse(value);
                string result = "其它";
                // 保存数值段
                foreach (string segment in p.Segments.Split(","))
                {
                    string[] segs = segment.Split("-");
                    // 获取数值范围
                    double begin = double.Parse(segs[0]);
                    double end = double.MaxValue;
                    if (segs.Length == 2)
                    {
                        end = double.Parse(segs[1]);
                    }
                    // 判断是否在范围内
                    if (val >= begin && val < end)
                    {
                        if (segs.Length == 1)
                        {
                            result = segs[0] + p.Unit + "以上";
                        }
                        else if (begin == 0)
                        {
                            result = segs[1] + p.Unit + "以下";
                        }
                        else
                        {
                            result = segment + p.Unit;
                        }
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion
    }
}
