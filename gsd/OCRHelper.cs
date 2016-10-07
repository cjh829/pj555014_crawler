using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tesseract;

namespace gsd
{
    public static class OCRHelper
    {
        public static string getText(string imgPath)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            using (var pix = Pix.LoadFromFile(imgPath))
            using (var page = engine.Process(pix))
            {
                return page.GetText().Trim();
            }
        }
    }
}
