using BL.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PageDetail.Controllers
{
    public class PageDetailController : Controller
    {
        private IPageDetailService _pageDetailService;
        public PageDetailController(IPageDetailService pageDetailService)
        {
            _pageDetailService = pageDetailService;
        }
        /// <summary>
        /// 生成详情页
        /// </summary>
		[Route("/item/{id}.html")]
        public IActionResult Index(long id)
        {
            var htmlmodel = _pageDetailService.LoadModel(id);

            return View(htmlmodel);
        }
    }
}
