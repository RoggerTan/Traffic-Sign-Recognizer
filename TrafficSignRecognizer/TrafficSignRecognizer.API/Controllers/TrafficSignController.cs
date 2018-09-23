using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        [HttpGet("get/{id}")]
        public ActionResult<string> Get(string id)
        {
            return new JsonResult(new {
                ImgUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/jpg/example.jpg",
                Name = "CẤM DỪNG XE VÀ ĐỖ",
                Type = "Biển báo cấm",
                Id = id,
                Description = "Để báo nơi cấm dừng xe và đỗ xe. Biển có hiệu lực cấm tất cả các loại xe cơ giới dừng và đỗ lại ở..."
            });
        }
    }
}
