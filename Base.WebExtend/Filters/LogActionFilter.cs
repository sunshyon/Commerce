using Base.Common.Models;
using Base.ThirdTool;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.WebExtend.Filters
{
    public class LogActionFilter: ActionFilterAttribute
    {
        private ILogger<LogActionFilter> _logger = null;
        private readonly EsClient _esClient;

        public LogActionFilter(ILogger<LogActionFilter> logger,
            EsClient esClient)
        {
            this._logger = logger;
            _esClient = esClient;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string url = context.HttpContext.Request.Path.Value;
            string argument = JsonConvert.SerializeObject(context.ActionArguments);
            string actionName = context.ActionDescriptor.DisplayName;
            this._logger.LogInformation($"url={url}-->action={actionName}-->argument={argument}");

            //写入分布式日志
            //var logModel = new LogModel()
            //{
            //    Type = LogType.Info,
            //    Time = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss")
            //    Url = url,
            //    MethodName = actionName
            //};
            //this._esClient.CreateIndex("commerce_log");
            //this._esClient.InsertOrUpdata<LogModel>(logModel);
        }
    }
}
