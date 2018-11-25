using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TrafficSignRecognizer.API.Models.Storages;
using TrafficSignRecognizer.Utils;
using TrafficSignRecognizer.Interfaces.Entities;
using static TrafficSignRecognizer.Utils.MatrixUtils;
using TrafficSignRecognizer.API.Models.ClassificationModel;
using TrafficSignRecognizer.API.Models.ANNModel.Utils;
using Microsoft.AspNetCore.Hosting;
using static TrafficSignRecognizer.Utils.BitmapUtils;

namespace TrafficSignRecognizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IHostingEnvironment _env;

        public TestController(IHostingEnvironment env)
        {
            _env = env;
        }

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

        [HttpPost("surfrect")]
        public IActionResult SurfRectangle([FromBody] Base64Image image)
        {
            if (image == null) return NoContent();

            var inputBitmap = image.Base64.ToBitmap();

            var surfModel = new SURFMatchingModel();

            foreach (var trainData in CNNModel.GetInstance("/DataSets/Training", "/DataSets/Testing", _env).DataSets.Train.TrainImages)
            {
                surfModel.Train(trainData.Id, GetBitmapFromBitmapUrl(trainData.ImgUrl, _env));
            }

            var croppedImages = surfModel.GetCroppedImages(inputBitmap);

            var results = croppedImages
                .Select(x => x.Resize(x.Width / 2, x.Width / 2).ToBase64Image().Base64).Take(1).ToArray();

            var results2 = croppedImages
    .Select(x => x.Resize(x.Width / 2, x.Width / 2).ToBase64Image().Base64).ToArray();

            return new JsonResult(new
            {
                results
            });
        }
    }
}