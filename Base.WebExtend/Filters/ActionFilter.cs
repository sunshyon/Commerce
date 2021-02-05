using Base.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.WebExtend.Filters
{
	public class ActionFilter : ActionFilterAttribute
	{
		private ILogger<ActionFilter> _logger = null;
		public ActionFilter(ILogger<ActionFilter> logger)
		{
			this._logger = logger;
		}
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			var nullRes = new  EmptyResult();//返回null
			if (context.Result.GetType() != nullRes.GetType() )
			{
				var result = ((ObjectResult)context.Result).Value;
				var resType = result.GetType();
				if (resType != typeof(AjaxResult))
				{
					context.Result = new JsonResult(new AjaxResult()
					{
						Message = "成功",
						Value = result,
						Result = true
					});
				}
			}
		}
		public override void OnResultExecuted(ResultExecutedContext context)
		{
		}
	}
}
