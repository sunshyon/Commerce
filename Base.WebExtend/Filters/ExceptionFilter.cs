using Base.Common.Models;
using Base.ThirdTool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.WebExtend.Filters
{
    public class ExceptionFilter: IExceptionFilter
    {
        private ILogger<ExceptionFilter> _logger = null;
        private readonly EsClient _esClient;

        public ExceptionFilter(ILogger<ExceptionFilter> logger,
            EsClient esClient)
        {
            this._logger = logger;
            _esClient = esClient;
        }
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                context.Result = new JsonResult(new AjaxResult()
                {
                    Message = "操作失败",
                    OtherValue = context.Exception.Message,
                    Result = false
                });
                this._logger.LogError(context.Exception.Message);
            }
            context.ExceptionHandled = true;

            //写入分布式日志
            string url = context.HttpContext.Request.Path.Value;
            var paramter = "";
            var rMethod= context.HttpContext.Request.Method;
            if (rMethod == "GET")
                paramter = context.HttpContext.Request.QueryString.Value;
            if (rMethod == "POST")
            {
                //需要在startup中添加以下中间件
                /*
                //用于过滤器中读取post body
                app.Use(next => context =>
                {
                    context.Request.EnableBuffering();
                    return next(context);
                });
                 */
                var req = context.HttpContext.Request;
                if (req.Body.Length > 0)
                {
                    req.Body.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(req.Body, Encoding.UTF8))
                    {
                        paramter = reader.ReadToEnd();
                    }
                }
            }
                
            string actionName = context.ActionDescriptor.DisplayName;
            var msg = context.Exception.Message;
            var fullST = context.Exception.StackTrace;
            var indexLine = fullST.IndexOf(":line");
            var secondAt = fullST.IndexOf("at", indexLine);
            var briefST = fullST.Substring(0, secondAt);
            var logModel = new LogModel()
            {
                Type = LogType.Error,
                Time = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss")
                Url = url,
                Parameter= paramter,
                MethodName = actionName,
                Msg=msg,
                StackTrace = briefST

            };
            //this._esClient.DropIndex("commerce_log");
            this._esClient.CreateIndex("commerce_log");
            this._esClient.InsertOrUpdata<LogModel>(logModel);
        }
    }
}
