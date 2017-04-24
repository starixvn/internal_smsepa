namespace SMS.EPA.Core.Utilities.NPOIExtensions
{
    using NPOI.SS.UserModel;
    using System.Web;

    public static class HSSFWorkbookExtensions
    {
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            return newSheet;
        }

        public static void ToReponseStream(this IWorkbook workbook, string fileName)
        {
            if (workbook != null)
            {
                var response = HttpContext.Current.Response;

                response.Clear();
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.xls");

                workbook.Write(response.OutputStream);
            }
        }
    }
}