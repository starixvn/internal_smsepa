namespace SMS.EPA.Services.Interfaces.ExportFile
{
    using System.Collections.Generic;
    using Model.Report;

    public interface IExportExcelService
    {
        string TemplatePath { get; set; }

        int RowStartHeader { get; set; }

        int RowStartBody { get; set; }

        int TotalColumns { get; set; }

        string FileName { get; set; }

        string SheetName { get; set; }

        void Export(IEnumerable<IExcelRecord> datas);
    }
}
