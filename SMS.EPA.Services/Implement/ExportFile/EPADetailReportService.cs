namespace SMS.EPA.Services.Implement
{
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;

    public class EPADetailReportService : IEPADetailReportService
    {
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

            var response = HttpContext.Current.Response;

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

            _sheet.AddMergedRegion(new CellRangeAddress(_currentRow, _currentRow, 0, _columnsName.Count));
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void WriteContentTable()
        {
            IFont redFont = _workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            ICellStyle evenStyle = CreateEvenStyle();
            ICellStyle oddStyle = CreateOddStyle();
            ICellStyle evenRedStyle = CreateEvenStyle();
            evenRedStyle.SetFont(redFont);
            ICellStyle oddRedStyle = CreateOddStyle();
            oddRedStyle.SetFont(redFont);

            var index = 1;
            foreach (var memberEPA in _epaDetail.MemberEPAs)
            {
                var currentColumn = 0;

                ICellStyle bodyStyle = _workbook.CreateCellStyle();
                bodyStyle = _currentRow % 2 == 0 ? evenStyle : oddStyle;

                IRow row = _sheet.CreateRow(_currentRow);
                dynamic score = CalculateScore(memberEPA);

                var cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(index++);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.Code);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.NickName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.FullName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.Category);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.GroupName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.JobTitleName);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Employee.JoinedDate.ToString(Constants.DateFormat));

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(memberEPA.Period.TargetTypeName);

                if (_currentRow % 2 == 0 && !memberEPA.Process.Self.IsEndProcess)
                {
                    bodyStyle = evenRedStyle;
                }
                else if (_currentRow % 2 != 0 && !memberEPA.Process.Self.IsEndProcess)
                {
                    bodyStyle = oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.SelfScore, (int)ProcessType.Self));

                if (_currentRow % 2 == 0 && !memberEPA.Process.Evaluators[0].IsEndProcess)
                {
                    bodyStyle = evenRedStyle;
                }
                else if (_currentRow % 2 != 0 && !memberEPA.Process.Evaluators[0].IsEndProcess)
                {
                    bodyStyle = oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator));

                if (_currentRow % 2 == 0 && !memberEPA.Process.Reviewers[0].IsEndProcess)
                {
                    bodyStyle = evenRedStyle;
                }
                else if (_currentRow % 2 != 0 && !memberEPA.Process.Reviewers[0].IsEndProcess)
                {
                    bodyStyle = oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer));

                if (_currentRow % 2 == 0 && !memberEPA.Process.Managements[0].IsEndProcess)
                {
                    bodyStyle = evenRedStyle;
                }
                else if (_currentRow % 2 != 0 && !memberEPA.Process.Managements[0].IsEndProcess)
                {
                    bodyStyle = oddRedStyle;
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(_epaDetail.PhaseSelecteds, score.ManagementScore.ToString(), (int)ProcessType.Management));

                bodyStyle = _currentRow % 2 == 0 ? evenStyle : oddStyle;

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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private dynamic CalculateScore(EPAModel memberEPA)
        {
            string disciplineScore = string.Empty,
                selfScore = string.Empty,
                evaluatorScore = string.Empty,
                reviewerScore = string.Empty,
                managementScore = string.Empty,
                finalScore = string.Empty,
                grade = string.Empty;

            if (memberEPA.HasDisciplineScore)
            {
                if (memberEPA.DisciplineScore < 0)
                {
                    memberEPA.DisciplineScore = 0;
                }

                disciplineScore = memberEPA.DisciplineScore.ToString();
            }
            else
            {
                if (memberEPA.DisciplineScore != 0)
                {
                    disciplineScore = memberEPA.DisciplineScore.ToString();
                }
                else
                {
                    disciplineScore = "-";
                }
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Self.StatusEndProcess != (int)ProcessStatusType.Open)
            {
                float score = memberEPA.Process.Self.TotalScore.Length > 0 ? float.Parse(memberEPA.Process.Self.TotalScore) : 0;

                selfScore = Math.Round(score, 2).ToString();
            }

            if (memberEPA.EPAId > 0 
                && memberEPA.Process.Evaluators != null
                && memberEPA.Process.Evaluators.Any()
                && memberEPA.Process.Evaluators[0].StatusEndProcess != (int)ProcessStatusType.Open)
            {
                evaluatorScore = Math.Round(memberEPA.Process.AverageScoreByEvaluators, 2).ToString();
            }

            if (memberEPA.EPAId > 0
                && memberEPA.Process.Reviewers != null
                && memberEPA.Process.Reviewers.Any()
                && memberEPA.Process.Reviewers[0].StatusEndProcess != (int)ProcessStatusType.Open)
            {
                reviewerScore = Math.Round(memberEPA.Process.AverageScoreByReviewers, 2).ToString();
            }

            if (memberEPA.EPAId > 0
                && memberEPA.Process.Managements != null
                && memberEPA.Process.Managements.Any()
                && memberEPA.Process.Managements[0].StatusEndProcess != (int)ProcessStatusType.Open)
            {
                managementScore = Math.Round(memberEPA.Process.AverageScoreByManagements, 2).ToString();
            }

            if (memberEPA.Period != null && memberEPA.Period.IsEndPeriod)
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

        private ICellStyle CreateOddStyle()
        {
            ICellStyle oddStyle = _workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateEvenStyle()
        {
            ICellStyle evenStyle = _workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
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
                "Management Score", 
                Lang.DisciplineScore, 
                Lang.FinalScore, 
                Lang.Grade 
            };
        }
    }
}
