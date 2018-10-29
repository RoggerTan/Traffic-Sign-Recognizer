using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TrafficSignRecognizer.API.Models.Storages;
using TrafficSignRecognizer.Utils;
using TrafficSignRecognizer.Interfaces.Entities;
using static TrafficSignRecognizer.Utils.MatrixUtils;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Data contains Base64Image img and Matrix filter</param>
        /// <returns></returns>
        [HttpPost("withfilter")]
        public IActionResult WithFilter([FromBody] dynamic data)
        {
            int[][] filter = data.filter.ToObject<int[][]>();
            Base64Image img = data.img.ToObject<Base64Image>();

            var filterMatrix = new Matrix<int>(filter, filter[0].Length, filter.Length);

            var convoluteResult = img.Base64
                .ToBitmap()
                .AsMatrix()
                .ConvoluteWith(filterMatrix);
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