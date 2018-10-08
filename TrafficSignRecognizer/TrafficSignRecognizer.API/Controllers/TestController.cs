using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TrafficSignRecognizer.API.Models.Storages;
using TrafficSignRecognizer.API.Models.Utils;
using TrafficSignRecognizer.Interfaces.Entities;
using static TrafficSignRecognizer.API.Models.Utils.MatrixUtils;

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

        [HttpPost("withfilter")]
        public IActionResult WithFilter([FromBody] Base64Image img)
        {
            var convoluteResult = img.Base64
                .ToBitmap()
                .AsMatrix()
                .ConvoluteWith(new int[][]{
                new[] {1, 1, 1},
                new[] {0, 0, 0},
                new[] {-1, -1, -1}
            });
            return new JsonResult(convoluteResult
                .ToBitmap()
                .ToBase64Image());
        }

        [HttpPost("tomatrix")]
        public IActionResult ToMatrix([FromBody] Base64Image img)
        {
            var bitmap = img.Base64
                .ToBitmap();
            return new JsonResult(new MatrixToken
            {
                Value = MatrixStorage.Add(bitmap
                .AsMatrix()),
                RowCount = bitmap.Height
            });
        }

        [HttpPost("getmatrix")]
        public IActionResult GetMatrix([FromBody] MatrixToken token)
        {
            var emurator = MatrixStorage.Get(token.Value).GetEnumerator();
            bool endRow = true;
            if (emurator.MoveNext())
                endRow = false;
            return new JsonResult(new MatrixToken.MatrixRow
            {
                Value = emurator.Current.ToArray(),
                EndRow = endRow
            });
        }
    }
}