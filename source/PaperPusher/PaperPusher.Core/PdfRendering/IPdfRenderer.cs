using System;

namespace PaperPusher.Core.PdfRendering
{
    public interface IPdfRenderer
    {
        /// <summary>
        /// The DPI density desired.
        /// </summary>
        int Density { get; set; }

        /// <summary>
        /// The generated image filename;
        /// </summary>
        string OutputFilename { get; set; }

        /// <summary>
        /// The page number of the PDF document to render.
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// Render an image of the given PDF document and save with the PreviewFilename.
        /// </summary>
        void Render(string filename);
    }
}
