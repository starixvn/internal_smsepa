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

        public ExportExcelService(string fileName, string sheetName, string title) : this()
        {
            FileName = fileName;
            SheetName = sheetName;
            Title = title;
        }

        public IWorkbook Workbook { get; set; }

        public ISheet Sheet { get; set; }

        public int TitleRowIndex { get; set; } = 0;

        public int HeaderRowIndex { get; set; } = 1;

        public int ContentRowStartIndex { get; set; } = 2;

        public int TotalColumns { get; set; }

        public string FileName { get; set; }

        public string SheetName { get; set; }

        public string Title { get; set; }

        public ICellStyle TitleStyle { get; set; }

        public IList<ICellStyle> HeaderStyles { get; set; }

        public IList<ICellStyle> ColumnsStyles { get; set; }

        public IList<ConditionalCellStyle> ConditionalCellStyles { get; set; }

        public void Init()
        {
            Workbook = new HSSFWorkbook();
            Sheet = Workbook.CreateSheet(SheetName);

            TitleStyle = Workbook.CreateCellStyle();
            HeaderStyles = new List<ICellStyle>();

            ConditionalCellStyles = new List<ConditionalCellStyle>();
            ColumnsStyles = new List<ICellStyle>();
        }

        private void WriteTitle()
        {
            IRow titleRow = Sheet.CreateRow(TitleRowIndex);
            ICell cell = titleRow.CreateCell(0, CellType.String);
            cell.SetCellValue(Title);
            cell.CellStyle = TitleStyle;
        }
        
        private void WriteHeaders(object sampleRecord)
        {
            Type recordType = sampleRecord.GetType();

            var columns = recordType.GetProperties();

            IRow headerRow = Sheet.CreateRow(HeaderRowIndex);

            for (var index = 0; index < columns.Length; index++)
            {
                PropertyInfo propertyInfo = columns[index];
                headerRow.CreateCell(index).SetCellValue(propertyInfo.Name);
                HeaderStyles.Add(Workbook.CreateCellStyle());
            }
        }

        private void WriteData(IEnumerable<IExcelRecord> datas)
        {
            int rowId = ContentRowStartIndex;

            foreach (IExcelRecord data in datas)
            {
                IRow row = Sheet.CreateRow(rowId);
                var objectData = data.ConvertToExcelRecord();
                PropertyInfo[] infos = objectData.GetType().GetProperties();
                for (int index = 0; index < infos.Length; index++)
                {
                    ICell cell = row.CreateCell(index);

                    var value = infos[index].GetValue(objectData, null);

                    cell.SetCellValue(value);


                    foreach (ConditionalCellStyle conditionalCellStyle in ConditionalCellStyles)
                    {
                        if (index == conditionalCellStyle.ColumnIndex && conditionalCellStyle.Condition(value))
                        {
                            cell.CellStyle = ColumnsStyles[conditionalCellStyle.StyleIndex];
                        }
                    }
                }

                rowId++;
            }
        }

        public int CreateNewColumnStyle()
        {
            if (ColumnsStyles != null)
            {
                ICellStyle style = Workbook.CreateCellStyle();
                ColumnsStyles.Add(style);
                return ColumnsStyles.IndexOf(style);
            }
            return 0;
        }
        
        public void Export(IEnumerable<IExcelRecord> datas)
        {
            if (datas == null || !datas.Any())
            {
                return;
            }

            WriteTitle();

            WriteHeaders(datas.First().ConvertToExcelRecord());

            WriteData(datas);

            int totalColumns = datas.First().GetType().GetProperties().Length;

            for (int i = 0; i < totalColumns; i++)
            {
                Sheet.AutoSizeColumn(i);
            }

            Sheet.CreateFreezePane(0, ContentRowStartIndex);

            Workbook.ToReponseStream(FileName);
        }
    }
}