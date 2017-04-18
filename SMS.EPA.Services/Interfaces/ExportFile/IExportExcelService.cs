namespace SMS.EPA.Services.Interfaces.ExportFile
{
    using System.Collections.Generic;
    using Model.Report;
    using NPOI.SS.UserModel;

    public interface IExportExcelService
    {
        IWorkbook Workbook { get; set; }

        ISheet Sheet { get; set; }

        int TitleRowIndex { get; set; }

        int HeaderRowIndex { get; set; }

        int ContentRowStartIndex { get; set; }

        int TotalColumns { get; set; }

        string FileName { get; set; }

        string SheetName { get; set; }

        string Title { get; set; }

        ICellStyle TitleStyle { get; set; }

        IList<ICellStyle> HeaderStyles { get; set; }

        IList<ICellStyle> ColumnsStyles { get; set; }

        /// <summary>
        /// Hold the condition to apply a style to a cell
        /// </summary>
        IList<ConditionalCellStyle> ConditionalCellStyles { get; set; }

        void Init();

        /// <summary>
        /// Return index of newly created style in ColumnsStyles list
        /// </summary>
        /// <returns></returns>
        int CreateNewColumnStyle();

        void Export(IEnumerable<IExcelRecord> datas);
    }
}
