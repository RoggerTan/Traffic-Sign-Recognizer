using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TrafficSignRecognizer.API.Models.ANNModel.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        [HttpPost("get")]
        public IActionResult Get([FromBody]Base64Image img)
        {
            if (img == null) return NoContent();
            var imgStream = new Bitmap(new MemoryStream(Convert.FromBase64String(img.Base64)));
            return new JsonResult(new {
                ImgUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/jpg/example.jpg",
                Name = "CẤM DỪNG XE VÀ ĐỖ",
                Type = "Biển báo cấm",
                Id = "123",
                Description = "Để báo nơi cấm dừng xe và đỗ xe. Biển có hiệu lực cấm tất cả các loại xe cơ giới dừng và đỗ lại ở..."
            });
        }

        [HttpGet]
        public IActionResult Predict([FromBody] Base64Image img, IHostingEnvironment env)
        {
            if (img == null) return NoContent();
            var imgStream = new Bitmap(new MemoryStream(Convert.FromBase64String(img.Base64)));

            var model = CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", env);

            if (!model.IsTrained)
            {
                model.BeginTraining(1000);
            }

            var result = model.Predict(imgStream);

            return new JsonResult(result);
        }
    }
}
