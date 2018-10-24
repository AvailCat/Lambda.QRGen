using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using ZXing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace QRGen
{
    public class Function
    {
        
        /// <summary>
        /// Create QRCode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            input.PathParameters.TryGetValue("url", out string url);
            input.Headers.TryGetValue("X-Forwarded-For", out string originIp);
            url = HttpUtility.UrlDecode(url);
            LambdaLogger.Log($"[URL] {url} from {originIp}");

            var watch = new Stopwatch();

            watch.Start();
            var w = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 256,
                    Width = 256,
                    Margin = 0,
                }
            };
            var image = Image.LoadPixelData<Rgba32>(w.Write(url).Pixels, 256, 256);
            var base64Str = ImageExtensions.ToBase64String(image, ImageFormats.Png);

            watch.Stop();
            LambdaLogger.Log($"[Generated] {base64Str.Length / 1024f}kb in {watch.Elapsed.TotalMilliseconds}ms");

            var respHeader = new Dictionary<string, string>
            {
                { "Content-Type", "image/png" },
                { "X-GenTime", $"{watch.Elapsed.TotalMilliseconds}ms" }
            };
            return new APIGatewayProxyResponse
            {
                Headers = respHeader,
                Body = base64Str.Replace("data:image/png;base64,", ""),
                StatusCode = 200,
                IsBase64Encoded = true
            };
        }
    }
}
