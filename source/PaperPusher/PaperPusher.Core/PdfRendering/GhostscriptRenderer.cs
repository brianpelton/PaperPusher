using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;

namespace PaperPusher.Core.PdfRendering
{
    public class GhostscriptRenderer : IPdfRenderer
    {
        public int Density { get; set; } = 150;
        public int Page { get; set; } = 1;
        public string PreviewFilename { get; set; }

        public void Render(string filename)
        {
            if (PreviewFilename == null)
                PreviewFilename = Path.GetTempFileName();

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(filename);

                var t = rasterizer.GetPage(Density, Density, Page);
                t.Save(PreviewFilename, ImageFormat.Png);
            }
        }
    }
}
