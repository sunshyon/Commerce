using Base.Common.Models;
using Base.ThirdTool;
using Base.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Base.WebExtend.Filters
{
    /// <summary>
    /// 防止重复提交
    /// </summary>
    public class RepeatSubmitFilter:ActionFilterAttribute
    {
        private static string KeyPrefix = "RepeatSubmitFilter";
        private readonly ILogger<ActionFilter> _logger;
        private readonly RedisClient _redisClient;

        public RepeatSubmitFilter(ILogger<ActionFilter> logger,
            RedisClient redisClient)
        {
            _logger = logger;
            _redisClient = redisClient;
        }

        /// <summary>
        /// 防重复提交周期  单位秒
        /// </summary>
        public int TimeOut = 3; //3秒有效期

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string url = context.HttpContext.Request.Path.Value;
            string argument = JsonSerializer.Serialize(context.ActionArguments);
            string ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            string agent = context.HttpContext.Request.Headers["User-Agent"];
            string sInfo = $"{url}-{argument}-{ip}-{agent}";
            string summary = MD5Helper.MD5EncodingOnly(sInfo);

            string totalKey = $"{KeyPrefix}-{summary}";

            string result = this._redisClient.StringGet(totalKey);
            if (string.IsNullOrEmpty(result))
            {
                this._redisClient.StringSet(totalKey, "1", TimeSpan.FromSeconds(TimeOut));
                this._logger.LogInformation($"RepeatSubmitFilter:{sInfo}");
            }
            else
            {
                //已存在
                this._logger.LogWarning($"RepeatSubmitFilter:{sInfo}");
                context.Result = new JsonResult(new AjaxResult()
                {
                    Result = false,
                    Message = $"请勿重复提交，{this.TimeOut}s之后重试"
                });
            }

            //CurrentUser currentUser = context.HttpContext.GetCurrentUserBySession();
            //if (currentUser == null)
            //{
            //    //if (this.IsAjaxRequest(context.HttpContext.Request))
            //    //{ }
            //    context.Result = new RedirectResult("~/Fourth/Login");
            //}
            //else
            //{
            //    this._logger.LogDebug($"{currentUser.Name} 访问系统");
            //}
        }
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
