using Base.Common.Consts;
using Base.ThirdTool;
using Base.Utility;
using BL.GoodsRelated;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Search;

namespace ESProcessor
{
    public class InitESIndexWorker : BackgroundService
    {
        private readonly ILogger<InitESIndexWorker> _logger;
        private readonly RabbitMQClient _rabbitMQ;
        private readonly EsClient _elasticSearch;
        private readonly SearchService _searchService;
        private readonly InitDataServcie _initDataServcie;

        public InitESIndexWorker(ILogger<InitESIndexWorker> logger, 
            RabbitMQClient rabbitMQ,
            EsClient elasticSearch,
            SearchService searchService,
            InitDataServcie initDataServcie
            )
        {
            _logger = logger;
            _rabbitMQ = rabbitMQ;
            _elasticSearch = elasticSearch;
            _searchService = searchService;
            _initDataServcie = initDataServcie;
        }
        /// <summary>
        /// 测试MQ和ES
        /// </summary>
        private void TestMQ_ES()
        {
            var exchange = RabbitMQConst.SKUCQRS_Exchange;
            var queue = RabbitMQConst.SKUCQRS_Queue_ESIndex;
            _rabbitMQ.Send(exchange, "Test");
            Thread.Sleep(500);
            _rabbitMQ.RegistReciveAction(exchange, queue, msg =>
            {
                Console.WriteLine(msg);
                var obj = new
                {
                    id = 1,
                    msg,
                    time = DateTime.Now
                };
                this._elasticSearch.InsertOrUpdata<object>(obj);
                return true;
            });
        }
        /// <summary>
        /// 先初始化数据，并发送到RabbitMQ
        /// </summary>
        private void InitData()
        {
            var exchange = RabbitMQConst.SKUCQRS_Exchange;
            var queue = RabbitMQConst.SKUCQRS_Queue_ESIndex;
            var spuIdList = _initDataServcie.InsertSpu(2);
            spuIdList.ForEach(id =>
            {
                Console.WriteLine($"新增SpuId={id}");
                var obj = new 
                {
                    spuId=id,
                    oType=1
                };
                _rabbitMQ.Send(exchange, obj.ToJson());
            });
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //初始化并发送数据
            InitData();
            //接受并写入数据
            var exchange = RabbitMQConst.SKUCQRS_Exchange;
            var queue = RabbitMQConst.SKUCQRS_Queue_ESIndex;
            _rabbitMQ.RegistReciveAction(exchange, queue, msg =>
            {
                try
                {
                    var obj = msg.JsonToT<dynamic>();
                    var spuId = (long)obj.spuId;
                    var oType = (byte)obj.oType; //1:增改，2:删

                    switch (oType)
                    {
                        case 1: //增改
                            {
                                Goods goods = this._searchService.GetGoodsBySpuId(spuId);
                                this._elasticSearch.InsertOrUpdata<Goods>(goods);
                                break;
                            }
                        case 2: //删除
                            this._elasticSearch.Delete<Goods>(spuId.ToString());
                            break;
                        default:
                            throw new Exception("wrong spuCQRSQueueModel.CQRSType");
                    }

                    this._logger.LogInformation($"{nameof(InitESIndexWorker)}.Init ESIndex succeed SpuId={spuId}");
                    return true;
                }
                catch (Exception ex)
                {
                    this._logger.LogError($"{nameof(InitESIndexWorker)}.Init ESIndex failed message={msg}, Exception:{ex.Message}");
                    return false;
                }
            });
            await Task.CompletedTask;
        }
    }
}
