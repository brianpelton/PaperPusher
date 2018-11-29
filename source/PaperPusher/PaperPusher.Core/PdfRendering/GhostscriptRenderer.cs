using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;

namespace PaperPusher.Core.PdfRendering
{
    public class GhostscriptRenderer : IPdfRenderer
    {
        public int Density { get; set; } = 150;
        public string OutputFilename { get; set; }
        public int PageNumber { get; set; } = 1;
        
        public void Render(string filename)
        {
            if (OutputFilename == null)
                OutputFilename = Path.GetTempFileName();

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(filename);

                var t = rasterizer.GetPage(Density, Density, PageNumber);
                t.Save(OutputFilename, ImageFormat.Png);
            }
        }
    }
}
