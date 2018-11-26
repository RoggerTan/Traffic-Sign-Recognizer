using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrafficSignRecognizer.API.Models.ANNModel.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        private IHostingEnvironment _Env;

        public TrafficSignController(IHostingEnvironment Env)
        {
            _Env = Env;
        }

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

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] Base64Image img)
        {
            if (img == null) return NoContent();
            var imgStream = new Bitmap(new MemoryStream(Convert.FromBase64String(img.Base64)));

            var model = CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", _Env);

            if (!model.IsTrained)
            {
                model.BeginTraining(9000);
            }

            var result = model.PredictMultiple(imgStream);

            return new JsonResult(new {
                result = result.ToArray()
            });
        }

        /// <summary>
        /// Get the network JSON data
        /// </summary>
        /// <returns></returns>
        [HttpGet("getnetwork")]
        public IActionResult GetSerializedNetwork()
        {
            var model = CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", _Env);

            if (!model.IsTrained)
            {
                model.BeginTraining(9000);
            }

            var ms = new MemoryStream();
            var streamWriter = new StreamWriter(ms);

            streamWriter.Write(model.GetSerializedNetwork());
            streamWriter.Close();

            return File(ms.ToArray(), "application/json", "data.json");
        }

        /// <summary>
        /// Receive JSON file (multipart) that contains network data.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("loadnetwork")]
        public async Task<IActionResult> LoadSerializedNetwork(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("No file is sent!");

            var fileStream = file.OpenReadStream();

            var model = CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", await new StreamReader(fileStream).ReadToEndAsync(),  _Env);

            if (!model.IsTrained)
            {
                model.BeginTraining(9000);
            }

            return new JsonResult(new { Result = "Ok"});
        }
    }
}
