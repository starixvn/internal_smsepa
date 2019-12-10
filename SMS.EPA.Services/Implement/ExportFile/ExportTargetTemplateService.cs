using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SMS.EPA.Core.Utilities.NPOIExtensions;
using SMS.EPA.Model;
using SMS.EPA.Services.Interfaces;

namespace SMS.EPA.Services.Implement
{
    public class ExportTargetTemplateService : IExportTargetTemplateService
    {
        private const int ContentStartRowIndex = 3;
        private const int NumOfFracDigits = 2;

        public IWorkbook Workbook { get; set; }
        public ISheet Sheet { get; set; }
        public string FileName { get; set; }
        public string SheetName { get; set; }
        public string TargetType { get; set; }
        public string TargetName { get; set; }
        public ICellStyle GroupNameStyle { get; set; }
        public ICellStyle GroupWeightStyle { get; set; }
        public ICellStyle KPIWeightStyle { get; set; }
        public IEnumerable<TargetTemplateKPIGroupModel> KPIIds { get; set; }

        public void Export(TargetTemplate targetTemplate, IEnumerable<TargetTemplateKPIGroupModel> kpis, string templatePath)
        {
            InitializeWithTemplate(templatePath);
            GetData(targetTemplate, kpis);

            WriteTitle();
            FillContent();

            Workbook.ToReponseStream(FileName);
        }

        private void GetData(TargetTemplate targetTemplate, IEnumerable<TargetTemplateKPIGroupModel> kpis)
        {
            KPIIds = kpis;
            TargetType = targetTemplate.TargetTypeName;
            TargetName = targetTemplate.Name;
            FileName = "KPI List - " + TargetName;
            SheetName = TargetName;

            GroupNameStyle = Sheet.GetRow(ContentStartRowIndex).GetCell(0).CellStyle;
            GroupWeightStyle = Sheet.GetRow(ContentStartRowIndex).GetCell(1).CellStyle;
            KPIWeightStyle = Sheet.GetRow(ContentStartRowIndex + 1).GetCell(1).CellStyle;
        }

        public void InitializeWithTemplate(string templatePath)
        {
            try
            {
                using (FileStream fireStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    Workbook = new HSSFWorkbook(fireStream, true);
                    Sheet = Workbook.GetSheetAt(0);
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void WriteTitle()
        {
            string title = "Name : " + TargetName;
            string subTitle = "Target Type : " + TargetType;

            Sheet.GetRow(0).GetCell(0).SetCellValue(title);
            Sheet.GetRow(1).GetCell(0).SetCellValue(subTitle);
        }

        private void FillContent()
        {
            int rowIndex = ContentStartRowIndex;

            foreach (var kpiGroup in KPIIds)
            {
                int colIndex = 0;
                var curRow = Sheet.CreateRow(rowIndex);

                var groupNameCell = curRow.CreateCell(colIndex++);
                groupNameCell.SetCellValue(kpiGroup.Name);
                groupNameCell.CellStyle = GroupNameStyle;

                var groupWeightCell = curRow.CreateCell(colIndex);
                groupWeightCell.SetCellValue(kpiGroup.Weight);
                groupWeightCell.CellStyle = GroupWeightStyle;

                rowIndex++;

                var selectedKPIs = kpiGroup.KPIs.Where(kpi => kpi.Selected == true);
                double spreadWeight = 0;
                spreadWeight = kpiGroup.IsSpreadKPI
                    ? Math.Round(kpiGroup.Weight.Value / selectedKPIs.Count(), NumOfFracDigits)
                    : 0;

                foreach (var kpi in selectedKPIs)
                {
                    colIndex = 0;
                    curRow = Sheet.CreateRow(rowIndex);

                    var nameCell = curRow.CreateCell(colIndex++);
                    nameCell.SetCellValue(kpi.Name);
                    var weightCell = curRow.CreateCell(colIndex);
                    weightCell.SetCellValue(kpiGroup.IsSpreadKPI ? spreadWeight : kpi.Weight);
                    weightCell.CellStyle = KPIWeightStyle;
                    rowIndex++;
                }
            }
        }
    }
}