using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        [HttpPost("get")]
        public IActionResult Get([FromBody]dynamic img)
        {
            if (img == null) return NoContent();
            var imgStream = new Bitmap(new MemoryStream(Convert.FromBase64String(img["base64"].Value)));
            return new JsonResult(new {
                ImgUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/jpg/example.jpg",
                Name = "CẤM DỪNG XE VÀ ĐỖ",
                Type = "Biển báo cấm",
                Id = "123",
                Description = "Để báo nơi cấm dừng xe và đỗ xe. Biển có hiệu lực cấm tất cả các loại xe cơ giới dừng và đỗ lại ở..."
            });
        }
    }
}
