using Microsoft.AspNetCore.Mvc;
using TrafficSignRecognizer.API.Models.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public IActionResult ToGrayScale([FromBody] Base64Image img)
        {
            return new JsonResult(img.Base64
                .ToBitmap()
                .ToGrayScale()
                .ToBase64Image());
        }
    }
}