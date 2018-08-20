using FinanceManagement.Utilities;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace FinanceManagement.StatementProcessing
{
    class PdfParser
    {
        internal static string[] ParsePDFStatement(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                string[] statement = new string[reader.NumberOfPages];
                for (int i = 0; i < reader.NumberOfPages; i++)
                {
                    statement[i] = PdfTextExtractor.GetTextFromPage(reader, i + 1);
                }
                Logger.Log($"Parsed PDF statement {path}.");
                return statement;
            }
        }
    }
}
