using BL.Contracts;
using Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GoodsRelated
{
    public class InitDataServcie:BaseService
    {
        private readonly OrangeContext _orangeContext;

        public InitDataServcie(OrangeContext orangeContext)
        {
            _orangeContext = orangeContext;
        }
        public List<long> InsertSpu(int index)
        {
            List<long> spuIdList = new List<long>();

            for (int i = 0; i < index; i++)
            {
                var tbSpu = new TbSpu();

                #region SPU
                tbSpu.Title = "华为 G9 青春版 " + i.ToString("00") + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss fff");
                tbSpu.SubTitle = "骁龙芯片！3GB运行内存！索尼1300万摄像头！" + i.ToString("00") + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + "<a href='https://sale.jd.com/act/DhKrOjXnFcGL.html' target='_blank'>华为新品全面上线，更多优惠猛戳》》</a>";
                tbSpu.Cid1 = 74;
                tbSpu.Cid2 = 75;
                tbSpu.Cid3 = 76;
                tbSpu.BrandId = 8557;
                tbSpu.Saleable = true;
                tbSpu.Valid = true;
                tbSpu.CreateTime = DateTime.Now;
                tbSpu.LastUpdateTime = DateTime.Now;
                _orangeContext.TbSpu.Add(tbSpu);
                _orangeContext.SaveChanges();
                #endregion

                #region SpuDetail
                var tbSpuDetail = new TbSpuDetail();
                tbSpuDetail.SpuId = tbSpu.Id; //todo 
                tbSpuDetail.Description = "<divclass='content_tpl'><divclass='formwork'><divclass='formwork_img'><imgsrc='//img20.360buyimg.com/vc/jfs/t5893/141/6838703316/1369626/15c9d88f/596c753aN075ee827.jpg'/></div></div></div><br/>";
                tbSpuDetail.GenericSpec = "{'1':'其它','2':'G9青春版（全网通版）','3':2016.0,'5':143,'6':'其它','7':'Android','8':'骁龙（Snapdragon)','9':'骁龙617（msm8952）','10':'八核','11':1.5,'14':5.2,'15':'1920*1080(FHD)','16':800.0,'17':1300.0,'18':3000.0}";
                tbSpuDetail.SpecialSpec = "{'4':['白色','金色','玫瑰金'],'12':['3GB'],'13':['16GB']}";
                tbSpuDetail.PackingList = "手机（电池内置）*1，中式充电器*1，数据线*1，半入耳式线控耳机*1，华为手机凭证*1，快速指南*1，取卡针*1，屏幕保护膜（出厂已贴）*1" + i.ToString("00") + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss fff");
                tbSpuDetail.AfterService = "本产品全国联保，享受三包服务，质保期为：一年质保" + i.ToString("00") + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss fff");

                _orangeContext.TbSpuDetail.Add(tbSpuDetail);
                _orangeContext.SaveChanges();
                #endregion
                {
                    #region Sku
                    var tbSku = new TbSku();
                    tbSku.SpuId = tbSpu.Id;// todo
                    tbSku.Title = "华为 G9 青春版 白色 移动联通电信4G手机 双卡双待" + i.ToString("00") + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss fff");
                    tbSku.Images = "http://yt.image.com/images/9/15/1524297313793.jpg";
                    tbSku.Price = 8400;
                    tbSku.Indexes = "0_0_0";
                    tbSku.OwnSpec = "{'4':'白色','12':'3GB','13':'16GB'}";
                    tbSku.Enable = true;
                    tbSku.CreateTime = DateTime.Now;
                    tbSku.LastUpdateTime = DateTime.Now;
                    _orangeContext.TbSku.Add(tbSku);
                    _orangeContext.SaveChanges();
                    #endregion

                    #region Stock
                    var tbStock = new TbStock();
                    tbStock.SkuId = tbSku.Id;
                    tbStock.Stock = 10000 + i + DateTime.Now.Millisecond;

                    _orangeContext.TbStock.Add(tbStock);
                    _orangeContext.SaveChanges();
                    #endregion
                }

                {
                    #region Sku
                    var tbSku = new TbSku();
                    tbSku.SpuId = tbSpu.Id;// todo
                    tbSku.Title = "华为 G9 青春版 白色 移动联通电信4G手机 双卡双待" + i.ToString("00") + "_____" + DateTime.Now.ToString("yyyyMMdd HHmmss fff");
                    tbSku.Images = "http://yt.image.com/images/9/15/1524297313793.jpg";
                    tbSku.Price = 8400;
                    tbSku.Indexes = "0_0_0";
                    tbSku.OwnSpec = "{'4':'白色','12':'3GB','13':'16GB'}";
                    tbSku.Enable = true;
                    tbSku.CreateTime = DateTime.Now;
                    tbSku.LastUpdateTime = DateTime.Now;
                    _orangeContext.TbSku.Add(tbSku);
                    _orangeContext.SaveChanges();
                    #endregion

                    #region Stock
                    var tbStock = new TbStock();
                    tbStock.SkuId = tbSku.Id;
                    tbStock.Stock = 10000 + i + DateTime.Now.Millisecond;

                    _orangeContext.TbStock.Add(tbStock);
                    _orangeContext.SaveChanges();
                    #endregion
                }

                spuIdList.Add(tbSpu.Id);
            }
        
            return spuIdList;
        }
    }
}
