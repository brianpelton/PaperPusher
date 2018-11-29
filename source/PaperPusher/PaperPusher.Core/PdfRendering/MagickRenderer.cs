using System;
using System.Linq;
using ImageMagick;

namespace PaperPusher.Core.PdfRendering
{
    public class MagickRenderer : IPdfRenderer
    {
        public int Density { get; set; } = 150;
        public string OutputFilename { get; set; }
        public int PageNumber { get; set; } = 1;

        public void Render(string filename)
        {
            var settings = new MagickReadSettings
            {
                Density = new Density(150, 150),
                FrameIndex = PageNumber - 1,
                FrameCount = 1,
            };

            using (var images = new MagickImageCollection())
            {
                images.Read(filename, settings);

                var image = images.First();
                image.Format = MagickFormat.Jpeg;
                images.Write(OutputFilename);
            }
        }
    }
}
