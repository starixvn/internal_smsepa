// ----------------------------------------------------------------------
// <copyright file="HSSFWorkbookExtensions.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="11/27/2014 3:02:15 PM">Created.</change>
// </history>

namespace NexFB.Core.Utilities.NPOIExtensions
{
    using System.Web;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Workbook extensions.
    /// </summary>
    public static class HSSFWorkbookExtensions
    {
        /// <summary>
        /// Clones sheet.
        /// </summary>
        /// <param name="workbook">Current workbook.</param>
        /// <param name="sourceSheetName">Source sheet name.</param>
        /// <param name="newSheetName">New sheet name.</param>
        /// <returns>Sheet instance.</returns>
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            return newSheet;
        }

        /// <summary>
        /// Exports Excel file to response stream.
        /// </summary>
        /// <param name="workbook">The workbook contains the data.</param>
        /// <param name="fileName">The file name.</param>
        public static void ToReponseStream(this IWorkbook workbook, string fileName)
        {
            var response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));

            workbook.Write(response.OutputStream);
        }
    }
}
