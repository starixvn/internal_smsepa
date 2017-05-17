namespace SMS.EPA.Core.Utilities
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;
    using System.IO;
    using System.Text;

    public static class PdfRender
    {
        private const int A4Width = 595;

        private const int A4Height = 842;

        public static byte[] ContentRender(string cssFilePath, string htmlText)
        {
            using (var stream = new MemoryStream())
            {
                using (var pdfDocument = new Document(new Rectangle(A4Width, A4Height), 5, 5, 40, 40))
                {
                    pdfDocument.SetPageSize(PageSize.A4);

                    var pdfWriter = PdfWriter.GetInstance(pdfDocument, stream);
                    var worker = XMLWorkerHelper.GetInstance();
                    var font = new UnicodeFontFactory();

                    pdfDocument.Open();

                    using (var htmlTextReader = new MemoryStream(Encoding.Unicode.GetBytes(htmlText)))
                    {
                        using (var cssStream = new FileStream(cssFilePath, FileMode.Open, FileAccess.Read))
                        {
                            worker.ParseXHtml(pdfWriter, pdfDocument, htmlTextReader, cssStream, Encoding.Unicode, font);
                        }
                    }

                    pdfDocument.Close();
                }

                return stream.ToArray();
            }
        }
    }
}