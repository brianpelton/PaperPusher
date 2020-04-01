using System;
using System.Drawing.Imaging;
using System.IO;
using IronPdf;

namespace PaperPusher.Core.PdfRendering
{
    public class IronPdfRenderer : IPdfRenderer
    {
        public int Density { get; set; } = 150;
        public string OutputFilename { get; set; }
        public int PageNumber { get; set; } = 1;

        public void Render(string filename)
        {
            if (OutputFilename == null)
                OutputFilename = Path.GetTempFileName();

            var rasterizer = PdfDocument.FromFile(filename);
            rasterizer.RasterizeToImageFiles(OutputFilename, new int[] { PageNumber }, ImageType.Png, Density);
        }
    }
}
