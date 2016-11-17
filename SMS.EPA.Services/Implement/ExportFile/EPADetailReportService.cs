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

        private HSSFWorkbook _workbook;
        private ISheet _sheet;
        private int _currentRow = 0;
        private IList<string> _columnsName;

        public EPADetailModel _epaDetail { get; set; }

        public MemoryStream Export(EPADetailModel epaDetail)
        {
            _epaDetail = epaDetail;
            _workbook = new HSSFWorkbook();
            _sheet = _workbook.CreateSheet(Lang.EPADetailReportSheet);
            _columnsName = CreateColumnsName();
            WriteTitleSheet();
            _currentRow += 2;

            WriteTitleTable();
            _currentRow++;

            WriteContentTable();
            _sheet.CreateFreezePane(0, 3);

            using (var output = new MemoryStream())
            {
                _workbook.Write(output);

                return output;
            }
        }

        public void Render(EPADetailModel epaDetail)
        {
            if (_workbook == null)
            {
                Export(epaDetail);
            }

            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", Lang.EPADetailReportName));

            _workbook.Write(response.OutputStream);
        }

        private void WriteTitleSheet()
        {
            IRow rowTitle = _sheet.CreateRow(_currentRow);

            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(Lang.EPADetailReportSheet);
            cell.CellStyle = _workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            _sheet.AddMergedRegion(new CellRangeAddress(_currentRow, _currentRow, 0, _columnsName.Count - 1));
        }

        private void WriteTitleTable()
        {
            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = _sheet.CreateRow(_currentRow);
            for (int i = 0; i < _columnsName.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(_columnsName[i]);
                _sheet.AutoSizeColumn(i);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void WriteContentTable()
        {
            IFont redFont = _workbook.CreateFont();
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
            foreach (var memberEPA in _epaDetail.EpaInfos)
            {
                var currentColumn = 0;

                ICellStyle bodyStyle = _workbook.CreateCellStyle();
                bodyStyle = _currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                IRow row = _sheet.CreateRow(_currentRow);
                dynamic score = CalculateScore(memberEPA);

                var cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(index++);
                SetColumnWidth(ref _sheet, currentColumn++, 5);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.Code);
                SetColumnWidth(ref _sheet, currentColumn++, 10);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.FullName);
                SetColumnWidth(ref _sheet, currentColumn++, 20);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.NickName);
                SetColumnWidth(ref _sheet, currentColumn++, 12);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.Category);
                SetColumnWidth(ref _sheet, currentColumn++, 12);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.GroupName);
                SetColumnWidth(ref _sheet, currentColumn++, 16);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.JobTitleName);
                SetColumnWidth(ref _sheet, currentColumn++, 25);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.JoinedDate.ToString(Constants.DateFormat));
                SetColumnWidth(ref _sheet, currentColumn++, 15);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleLeft : oddStyleLeft;

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Period.TargetTypeName);

                bodyStyle = _currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                if (!memberEPA.Process.Self.IsEndProcess)
                {
                    bodyStyle = _currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.SelfScore, (int)ProcessType.Self));

                if (!memberEPA.Process.Evaluators.First().IsEndProcess)
                {
                    bodyStyle = _currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator));

                if (!memberEPA.Process.Reviewers.First().IsEndProcess)
                {
                    bodyStyle = _currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer));

                if (memberEPA.Process.Managements.Any() && !memberEPA.Process.Managements.First().IsEndProcess)
                {
                    bodyStyle = _currentRow % 2 == 0 ? evenRedStyle : oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.ManagementScore.ToString(), (int)ProcessType.Management));

                bodyStyle = _currentRow % 2 == 0 ? evenStyleCenter : oddStyleCenter;

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.DisciplineScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.Grade);

                _currentRow++;
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
        private dynamic CalculateScore(EPAModel memberEPA)
        {
            string disciplineScore = string.Empty,
                selfScore = string.Empty,
                evaluatorScore = string.Empty,
                reviewerScore = string.Empty,
                managementScore = string.Empty,
                finalScore = string.Empty,
                grade = string.Empty;

            float? score = null;

            if (memberEPA.HasDisciplineScore)
            {
                disciplineScore = memberEPA.DisciplineScore < 0 ? "0" : memberEPA.DisciplineScore.ToString();
            }
            else
            {
                disciplineScore = memberEPA.DisciplineScore != 0 ? memberEPA.DisciplineScore.ToString() : "-";
            }

            if (memberEPA.EPAId > 0)
            {
                switch (memberEPA.Process.Self.StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        score = !string.IsNullOrEmpty(memberEPA.Process.Self.TotalScore) ? float.Parse(memberEPA.Process.Self.TotalScore) : 0;
                        selfScore = Math.Round(score.Value, 2).ToString();
                        break;

                    case (int)ProcessStatusType.Closed:
                        selfScore = !string.IsNullOrEmpty(memberEPA.Process.Self.TotalScore)
                            ? Math.Round(float.Parse(memberEPA.Process.Self.TotalScore), 2).ToString() : Lang.NotAvailable;
                        break;
                }

                switch (memberEPA.Process.Evaluators.First().StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        evaluatorScore = memberEPA.Process.AverageScoreByEvaluators.HasValue
                            ? memberEPA.Process.AverageScoreByEvaluators.Value.ToString() : "0";
                        break;

                    case (int)ProcessStatusType.Closed:
                        evaluatorScore = memberEPA.Process.AverageScoreByEvaluators.HasValue
                            ? memberEPA.Process.AverageScoreByEvaluators.Value.ToString() : Lang.NotAvailable;
                        break;
                }

                switch (memberEPA.Process.Reviewers.First().StatusEndProcess)
                {
                    case (int)ProcessStatusType.InProgress:
                        reviewerScore = memberEPA.Process.AverageScoreByReviewers.HasValue
                            ? memberEPA.Process.AverageScoreByReviewers.Value.ToString() : "0";
                        break;

                    case (int)ProcessStatusType.Closed:
                        reviewerScore = memberEPA.Process.AverageScoreByReviewers.HasValue
                            ? memberEPA.Process.AverageScoreByReviewers.Value.ToString() : Lang.NotAvailable;
                        break;
                }

                if (memberEPA.Process.Managements.Any())
                {
                    switch (memberEPA.Process.Reviewers.First().StatusEndProcess)
                    {
                        case (int)ProcessStatusType.InProgress:
                            managementScore = memberEPA.Process.AverageScoreByManagements.HasValue
                                ? memberEPA.Process.AverageScoreByManagements.Value.ToString() : "0";
                            break;

                        case (int)ProcessStatusType.Closed:
                            managementScore = memberEPA.Process.AverageScoreByManagements.HasValue
                                ? memberEPA.Process.AverageScoreByManagements.Value.ToString() : Lang.NotAvailable;
                            break;
                    }
                }

                if (memberEPA.IsSemiHasMonthly)
                {
                    selfScore = memberEPA.MonthlyEPA.AverageSelfScoreConverted.HasValue
                        ? memberEPA.MonthlyEPA.AverageSelfScoreConverted.Value.ToString() : Lang.NotAvailable;

                    evaluatorScore = memberEPA.MonthlyEPA.AverageEvaluatorScoreConverted.HasValue
                        ? memberEPA.MonthlyEPA.AverageEvaluatorScoreConverted.Value.ToString() : Lang.NotAvailable;
                }
            }

            if (memberEPA.Period.IsEndPeriod)
            {
                if (string.IsNullOrEmpty(memberEPA.FinalScore))
                {
                    finalScore = Lang.NotAvailable;
                    grade = Lang.NotAvailable;
                }
                else
                {
                    finalScore = Math.Round(float.Parse(memberEPA.FinalScore), 2).ToString();
                    grade = memberEPA.Grade;
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
            ICellStyle oddStyle = _workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateOddStyleLeft()
        {
            ICellStyle oddStyle = _workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Left;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateEvenStyleCenter()
        {
            ICellStyle evenStyle = _workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        private ICellStyle CreateEvenStyleLeft()
        {
            ICellStyle evenStyle = _workbook.CreateCellStyle();
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

        private void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}