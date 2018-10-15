using BlazorUtils.Dev;
using BlazorUtils.Interfaces.EventArgs;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using System.Net.Http;
using TrafficSignRecognizer.Interfaces.Entities;
using TrafficSignRecognizer.Web.Entities.InjectTypes;

namespace TrafficSignRecognizer.Web.Pages.Test
{
    public class TestLogic : DevComponent
    {
        [Inject]
        protected HttpClient httpClient { get; set; }

        protected netstandard2._0.BlazorUtilsComponents.LMTInjector<Api> apiInfo;
        protected Base64Image grayscaledImage;
        protected int[][] grayscaledImageMatrix;
        protected MatrixToken matrixToken;
        protected netstandard2._0.BlazorUtilsComponents.LMTLocal _mainLocal;
        protected int currentRow = -1;
        protected Base64Image filteredImage;

        protected async void ImageDropHandler(LMTDropEventArgs e)
        {
            var base64Image = new Base64Image { Base64 = e.Base64String };

            //grayscale
            grayscaledImage = await httpClient.PostJsonAsync<Base64Image>($"{apiInfo.Get().TestUrl}/tograyscale", base64Image);

            //filter
            var filter = new int[][]{
            new []{ 1, 1, 1},
            new [] {0, 0, 0},
            new []{-1, -1, -1}
        };
            filteredImage = await httpClient.PostJsonAsync<Base64Image>($"{apiInfo.Get().TestUrl}/withfilter", new { img = base64Image, filter });

            StateHasChanged();

            #region get grayscaled image matrix
            //matrixToken = await httpClient.PostJsonAsync<MatrixToken>($"{apiInfo.Get().TestUrl}/tomatrix", grayscaledImage);

            //grayscaledImageMatrix = new int[matrixToken.RowCount][];

            //for (var CurRow = 0; CurRow < matrixToken.RowCount; CurRow++)
            //{
            //    grayscaledImageMatrix[CurRow] = (await httpClient.PostJsonAsync<MatrixToken.MatrixRow>($"{apiInfo.Get().TestUrl}/getmatrix", matrixToken)).Value;
            //    currentRow++;
            //    await Task.Delay(500);
            //    StateHasChanged();
            //}
            #endregion

            Dev.Warn("Done!");
        }
    }
}
