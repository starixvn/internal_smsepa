namespace SMS.EPA.Services.Implement.ExportFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Utilities.NPOIExtensions;
    using Interfaces.ExportFile;
    using Model.Report;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public class ExportExcelService : IExportExcelService
    {
        public ExportExcelService()
        {
        }

        public ExportExcelService(string templatePath, string fileName, string sheetName, int totalColumns)
        {
            TemplatePath = templatePath;
            FileName = fileName;
            SheetName = sheetName;
            TotalColumns = totalColumns;
        }

        public string TemplatePath { get; set; }

        public int RowStartHeader { get; set; } = 2;

        public int RowStartBody { get; set; } = 3;

        public int TotalColumns { get; set; }

        public string FileName { get; set; }

        public string SheetName { get; set; }
        
        public void Export(IEnumerable<IExcelRecord> datas)
        {
            HSSFWorkbook wordbook = new HSSFWorkbook();
            ISheet sheet = wordbook.CreateSheet(SheetName);

            var sampleRecord = datas.FirstOrDefault()?.ConvertToExcelRecord();
            Type recordType = sampleRecord?.GetType();
            if (recordType != null)
            {
                var columns = recordType.GetProperties();
                IRow headerRow = sheet.CreateRow(0);
                for (var index = 0; index < columns.Length; index++)
                {
                    PropertyInfo propertyInfo = columns[index];
                    headerRow.CreateCell(index).SetCellValue(propertyInfo.Name);
                    sheet.AutoSizeColumn(index);
                }
            }

            int rowId = 1;

            foreach (IExcelRecord data in datas)
            {
                IRow row = sheet.CreateRow(rowId);
                var objectData = data.ConvertToExcelRecord();
                PropertyInfo[] infos = objectData.GetType().GetProperties();
                for (int index = 0; index < infos.Length; index++)
                {
                    ICell cell = row.CreateCell(index);
                    cell.SetCellValue(infos[index].GetValue(objectData, null));
                }

                rowId++;
            }

            sheet.CreateFreezePane(0, RowStartBody);

            wordbook.ToReponseStream(FileName);
        }
    }
}
