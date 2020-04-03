using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.Controllers
{
    /// <summary>
    /// 订单控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// 订单是否存在
        /// </summary>
        /// <param name="id">必须可以转为long</param>
        /// <returns></returns>
        //[HttpGet("{id:MyRouteConstraint}")]
        [HttpGet("{id:isLong}")]
        public bool OrderExist([FromRoute]string id)
        {
            return true;
        }

        /// <summary>
        /// 订单最大数量
        /// </summary>
        /// <param name="id">最大20</param>
        /// <param name="linkGenerator"></param>
        /// <returns></returns>
        [HttpGet("{id:max(20)}")]
        public bool Max(long id, [FromServices] LinkGenerator linkGenerator)
        {
            var a = linkGenerator.GetPathByAction("Reque", "Order");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <para name="ss">必填</para>
        /// <returns></returns>
        [HttpGet("{name:required}")]
        [Obsolete]
        public bool Reque(string name)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number">以三个数字开始</param>
        /// <returns></returns>
        [HttpGet("{number:regex(^\\d{{3}}$)}")]
        public bool Number(string number)
        {
            return true;
        }
    }
}