using SkiaSharp;
using ZXing;
using ZXing.Common;

namespace ComedorEstudiantil.Web.Services
{
    public class CodigoBarrasService : ICodigoBarrasService
    {
        public byte[] GenerarPng(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException(
                    "El código de barras es obligatorio.",
                    nameof(codigo));
            }

            var escritor =
                new ZXing.SkiaSharp.BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Width = 500,
                        Height = 140,
                        Margin = 12,
                        PureBarcode = true
                    }
                };

            using SKBitmap imagen =
                escritor.Write(codigo.Trim());

            using SKData datos =
                imagen.Encode(
                    SKEncodedImageFormat.Png,
                    100);

            return datos.ToArray();
        }
    }
}