namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class EPADetailReportService : IEPADetailReportService
    {
        private const int ExcelColumnWidth = 256;

        private HSSFWorkbook workbook;
        private ISheet sheet;
        private int currentRow = 0;
        private IList<string> columnsName;

        public EPADetailModel EpaDetail { get; set; }

        public MemoryStream Export(EPADetailModel epaDetail)
        {
            EpaDetail = epaDetail;
            workbook = new HSSFWorkbook();
            sheet = workbook.CreateSheet(Lang.EPADetailReportSheet);
            columnsName = CreateColumnsName();
            WriteTitleSheet();
            currentRow += 2;

            WriteTitleTable();
            currentRow++;

            WriteContentTable();
            sheet.CreateFreezePane(0, 3);

            using (var output = new MemoryStream())
            {
                workbook.Write(output);

                return output;
            }
        }

        public void Render(EPADetailModel epaDetail)
        {
            if (workbook == null)
            {
                Export(epaDetail);
            }

            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", Lang.EPADetailReportName));

            workbook.Write(response.OutputStream);
        }

        private void WriteTitleSheet()
        {
            IRow rowTitle = sheet.CreateRow(currentRow);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(Lang.EPADetailReportSheet);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, columnsName.Count - 1));
        }

        private void WriteTitleTable()
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(currentRow);
            for (int i = 0; i < columnsName.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(columnsName[i]);
                sheet.AutoSizeColumn(i);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void WriteContentTable()
        {
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            ICellStyle evenStyleCenter = CreateEvenStyleCenter();
            ICellStyle oddStyleCenter = CreateOddStyleCenter();
            ICellStyle evenRedStyle = CreateEvenStyleCenter();
            evenRedStyle.SetFont(redFont);
            ICellStyle oddRedStyle = CreateOddStyleCenter();
            oddRedStyle.SetFont(redFont);
            ICellStyle evenStyleLeft = CreateEvenStyleLeft();
            ICellStyle oddStyleLeft = CreateOddStyleLeft();

            var index = 1;
            foreach (var epaInfo in EpaDetail.EpaInfos)
            {
                var currentColumn = 0;

                ICellStyle bodyStyle = workbook.CreateCellStyle();
                bodyStyle = currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                IRow row = sheet.CreateRow(currentRow);
                dynamic score = CalculateScore(epaInfo);

                var cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(index++);
                SetColumnWidth(currentColumn++, 5);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.Code);
                SetColumnWidth(currentColumn++, 10);

                bodyStyle = currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.FullName);
                SetColumnWidth(currentColumn++, 20);

                bodyStyle = currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.NickName);
                SetColumnWidth(currentColumn++, 12);

                bodyStyle = currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.Category);
                SetColumnWidth(currentColumn++, 12);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.GroupName);
                SetColumnWidth(currentColumn++, 16);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.JobTitleName);
                SetColumnWidth(currentColumn++, 25);

                bodyStyle = currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.JoinedDate.ToString(Constants.DateFormat));
                SetColumnWidth(currentColumn++, 15);

                bodyStyle = currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Period.TargetTypeName);

                bodyStyle = currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                if (!epaInfo.Process.Self.IsEndProcess)
                {
                    bodyStyle = currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.SelfScore, (int)ProcessType.Self));

                if (!epaInfo.Process.Evaluators.First().IsEndProcess)
                {
                    bodyStyle = currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator));

                if (!epaInfo.Process.Reviewers.First().IsEndProcess)
                {
                    bodyStyle = currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer));

                if (epaInfo.Process.Managements.Any() && !epaInfo.Process.Managements.First().IsEndProcess)
                {
                    bodyStyle = currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.ManagementScore.ToString(), (int)ProcessType.Management));

                bodyStyle = currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.DisciplineScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.Grade);

                currentRow++;
            }
        }

        private string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            return phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1) ? score : "-";
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private dynamic CalculateScore(EPAModel epaInfo)
        {
            string disciplineScore = string.Empty,
                selfScore = string.Empty,
                evaluatorScore = string.Empty,
                reviewerScore = string.Empty,
                managementScore = string.Empty,
                finalScore = string.Empty,
                grade = string.Empty;

            float? score = null;

            if (epaInfo.HasDisciplineScore)
            {
                disciplineScore = epaInfo.DisciplineScore < 0 ? "0" : epaInfo.DisciplineScore.ToString();
            }
            else
            {
                disciplineScore = epaInfo.DisciplineScore != 0 ? epaInfo.DisciplineScore.ToString() : "-";
            }

            if (epaInfo.EPAId > 0)
            {
                switch (epaInfo.Process.Self.StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        score = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore) ? float.Parse(epaInfo.Process.Self.TotalScore) : 0;
                        selfScore = Math.Round(score.Value, 2).ToString();
                        break;

                    case (int)ProcessStatusType.Closed:
                        selfScore = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore)
                            ? Math.Round(float.Parse(epaInfo.Process.Self.TotalScore), 2).ToString() : Lang.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Evaluators.First().StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        evaluatorScore = epaInfo.Process.AverageScoreByEvaluators.HasValue
                            ? epaInfo.Process.AverageScoreByEvaluators.Value.ToString() : "0";
                        break;

                    case (int)ProcessStatusType.Closed:
                        evaluatorScore = epaInfo.Process.AverageScoreByEvaluators.HasValue
                            ? epaInfo.Process.AverageScoreByEvaluators.Value.ToString() : Lang.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Reviewers.First().StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        reviewerScore = epaInfo.Process.AverageScoreByReviewers.HasValue
                            ? epaInfo.Process.AverageScoreByReviewers.Value.ToString() : "0";
                        break;

                    case (int)ProcessStatusType.Closed:
                        reviewerScore = epaInfo.Process.AverageScoreByReviewers.HasValue
                            ? epaInfo.Process.AverageScoreByReviewers.Value.ToString() : Lang.NotAvailable;
                        break;
                }

                if (epaInfo.Process.Managements.Any())
                {
                    switch (epaInfo.Process.Reviewers.First().StatusEndProcess)
                    {
                        case (int)ProcessStatusType.InProgress:
                            managementScore = epaInfo.Process.AverageScoreByManagements.HasValue
                                ? epaInfo.Process.AverageScoreByManagements.Value.ToString() : "0";
                            break;

                        case (int)ProcessStatusType.Closed:
                            managementScore = epaInfo.Process.AverageScoreByManagements.HasValue
                                ? epaInfo.Process.AverageScoreByManagements.Value.ToString() : Lang.NotAvailable;
                            break;
                    }
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    selfScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted.HasValue
                        ? epaInfo.MonthlyEPA.AverageSelfScoreConverted.Value.ToString() : Lang.NotAvailable;

                    evaluatorScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.HasValue
                        ? epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.Value.ToString() : Lang.NotAvailable;
                }
            }

            if (epaInfo.Period.IsEndPeriod)
            {
                if (string.IsNullOrEmpty(epaInfo.FinalScore))
                {
                    finalScore = Lang.NotAvailable;
                    grade = Lang.NotAvailable;
                }
                else
                {
                    finalScore = Math.Round(float.Parse(epaInfo.FinalScore), 2).ToString();
                    grade = epaInfo.Grade;
                }
            }
            else
            {
                finalScore = Lang.Pending;
                grade = Lang.Pending;
            }

            return new
            {
                SelfScore = selfScore,
                EvaluatorScore = evaluatorScore,
                ReviewerScore = reviewerScore,
                ManagementScore = managementScore,
                DisciplineScore = disciplineScore,
                FinalScore = finalScore,
                Grade = grade
            };
        }

        private ICellStyle CreateOddStyleCenter()
        {
            ICellStyle oddStyle = workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateOddStyleLeft()
        {
            ICellStyle oddStyle = workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Left;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateEvenStyleCenter()
        {
            ICellStyle evenStyle = workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        private ICellStyle CreateEvenStyleLeft()
        {
            ICellStyle evenStyle = workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Left;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        private IList<string> CreateColumnsName()
        {
            return new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Category,
                Lang.Section,
                Lang.JobTitle,
                Lang.GroupDate,
                Lang.TargetType,
                Lang.SelfScore,
                Lang.EvaluationScore,
                Lang.ReviewerScore,
                Lang.ManagementScore,
                Lang.DisciplineScore,
                Lang.FinalScore,
                Lang.Grade
            };
        }

        private void SetColumnWidth(int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}