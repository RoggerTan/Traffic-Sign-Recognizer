using Microsoft.AspNetCore.Mvc;
using TrafficSignRecognizer.API.Models.Storages;
using TrafficSignRecognizer.API.Models.Utils;
using TrafficSignRecognizer.Interfaces.Entities;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("tograyscale")]
        public IActionResult ToGrayScale([FromBody] Base64Image img)
        {
            return new JsonResult(img.Base64
                .ToBitmap()
                .ToGrayScale()
                .ToBase64Image());
        }

        [HttpPost("tomatrix")]
        public IActionResult ToMatrix([FromBody] Base64Image img)
        {
            var bitmap = img.Base64
                .ToBitmap();
            return new JsonResult(new MatrixToken
            {
                Value = MatrixStorage.Add(new ImageMatrix
                {
                    Value = bitmap
                .AsMatrix(),
                    Width = bitmap.Width,
                    Height = bitmap.Height
                }),
                RowCount = bitmap.Height,
                ColCount = bitmap.Width,
                CurrentRow = -1
            });
        }

        [HttpPost("getmatrix")]
        public IActionResult GetMatrix([FromBody] MatrixToken token)
        {
            return new JsonResult(new MatrixToken.MatrixRow
            {
                Value = MatrixStorage.Get(token.Value).Value[token.CurrentRow]
            });
        }
    }
}