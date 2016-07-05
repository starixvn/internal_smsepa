namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using NexFB.Core.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class ReportService : IReportService
    {
        private const int ExcelColumnWidth = 256;
        private readonly IReportRepository reportRepository;
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IEmployeeEPAService employeeEPAService;

        public ReportService()
            : this(null, null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService employeeEPAService)
        {
            this.reportRepository = reportRepository ?? new ReportRepository();
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public DashboardModel GetDashboard(GetDashboardCriteria criteria)
        {
            DashboardModel result = new DashboardModel();

            if (criteria != null && criteria.IsValid())
            {
                result = employeeEPARepository.GetDashboard(criteria);
            }

            return result;
        }

        #region EPA Summary Report

        public EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            EPASummaryModel result = new EPASummaryModel();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetEPASummary(criteria);
            }

            return result;
        }

        public MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.EPASummaryReportTitle);
        }

        #endregion EPA Summary Report

        #region EPA Detail Report

        public EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria)
        {
            EPADetailModel result = new EPADetailModel();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetEPADetail(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", result.MemberEPAs.Where(x => x.HasMonthlyEPA && x.Period.TargetTypeId == (int)TargetType.SemiAnnual).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = employeeEPAService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel epaInfo in result.MemberEPAs)
                {
                    employeeEPAService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    employeeEPAService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return result;
        }

        #endregion EPA Detail Report

        #region Monthly EPA Detail Report

        public MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.MonthlyEPASummaryReportTitle);
        }

        public EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            EPADetailModel result = new EPADetailModel();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetMonthlyEPADetail(criteria);
            }

            return result;
        }

        public void ExportMonthlyEPADetailReport(EPADetailModel epaDetail, string templatePath)
        {
            string reportName = Lang.MonthlyEPADetailReportName;
            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.MonthlyEPADetailReportSheet);

            writer.CopyRows(0, 3);

            FillContentRowMonthlyEPADetailReport(epaDetail, ref writer);
            writer.SyncColumnsWidth(11);

            writer.Render();
        }

        public EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            EPASummaryModel result = new EPASummaryModel();

            if (criteria != null && criteria.IsValid())
            {
                result = reportRepository.GetMonthlyEPASummary(criteria);
            }

            return result;
        }

        #endregion Monthly EPA Detail Report

        private void RenderHeaderEPASummaryReport(
            IList<PeriodModel> periodSelecteds,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeaders)
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

            IRow row = sheet.CreateRow(2);
            for (int i = 0; i < titleHeaders.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleHeaders[i]);

                sheet.AddMergedRegion(new CellRangeAddress(2, 3, i, i));
            }

            for (int i = 0; i < periodSelecteds.Count; i++)
            {
                var cell = row.CreateCell(titleHeaders.Count + (2 * i));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(periodSelecteds[i].Name);

                sheet.AddMergedRegion(new CellRangeAddress(2, 2, titleHeaders.Count + (2 * i), titleHeaders.Count + ((2 * i) + 1)));
            }

            IRow rowPeriod = sheet.CreateRow(3);
            for (int i = 0; i < periodSelecteds.Count; i++)
            {
                var cell = rowPeriod.CreateCell(titleHeaders.Count + (2 * i));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(Lang.FinalScore);

                cell = rowPeriod.CreateCell(titleHeaders.Count + ((2 * i) + 1));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(Lang.Grade);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void RenderContentRowEPASummaryReport(
            EPASummaryModel epaSummary,
            ref HSSFWorkbook workbook,
            ref ISheet sheet)
        {
            var rowIndex = 4;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.Center;
            bodyStyle1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.Center;
            bodyStyle2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.Left;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            ICell cellBody;

            foreach (var epaInfo in epaSummary.EPAInfos)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                IRow row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 3);
                SetColumnWidth(ref sheet, 0, 5);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.Code);
                SetColumnWidth(ref sheet, 1, 10);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(2);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.FullName);
                SetColumnWidth(ref sheet, 2, 20);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.NickName);
                SetColumnWidth(ref sheet, 3, 12);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle4;
                }
                else
                {
                    bodyStyle = bodyStyle3;
                }

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.Category);
                SetColumnWidth(ref sheet, 4, 12);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.Group.GroupName);
                SetColumnWidth(ref sheet, 5, 16);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.JobTitle.JobTitleName);
                SetColumnWidth(ref sheet, 6, 25);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaInfo.JoinedDate.ToString(Constants.DateFormat));
                SetColumnWidth(ref sheet, 7, 25);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                List<int> periodIds = epaInfo.Periods.Select(x => x.PeriodId).ToList();
                for (int i = 0; i < epaSummary.PeriodSelecteds.Count; i++)
                {
                    if (periodIds.Contains(epaSummary.PeriodSelecteds[i].PeriodId))
                    {
                        string finalScore = string.Empty,
                            grade = string.Empty;
                        EPASummaryPeriodModel period = epaInfo.Periods.FirstOrDefault(x => x.PeriodId == epaSummary.PeriodSelecteds[i].PeriodId);

                        if (period != null && period.IsEndPeriod.Equals(true))
                        {
                            if (string.IsNullOrEmpty(period.FinalScore))
                            {
                                finalScore = Lang.NotAvailable;
                                grade = Lang.NotAvailable;
                            }
                            else
                            {
                                finalScore = Math.Round(float.Parse(period.FinalScore), 2).ToString();
                                grade = period.Grade;
                            }
                        }
                        else
                        {
                            finalScore = Lang.Pending;
                            grade = Lang.Pending;
                        }

                        cellBody = row.CreateCell(8 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(finalScore);
                        SetColumnWidth(ref sheet, 8 + (2 * i), 14);

                        cellBody = row.CreateCell(8 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(grade);
                        SetColumnWidth(ref sheet, 8 + ((2 * i) + 1), 10);
                    }
                    else
                    {
                        cellBody = row.CreateCell(8 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 8 + (2 * i), 14);

                        cellBody = row.CreateCell(8 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 8 + ((2 * i) + 1), 10);
                    }
                }

                rowIndex++;
            }
        }

        private void FillContentRowEPADetailReport(
            EPADetailModel epaDetail,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EPAModel memberEPA in epaDetail.MemberEPAs)
            {
                FillRowEPADetailReport(memberEPA, epaDetail.PhaseSelecteds, ref writer, ref rowIndex);
            }
        }

        private void FillContentRowMonthlyEPADetailReport(
            EPADetailModel epaDetail,
            ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EPAModel memberEPA in epaDetail.MemberEPAs)
            {
                FillRowMonthlyEPADetailReport(memberEPA, epaDetail.PhaseSelecteds, ref writer, ref rowIndex);
            }
        }

        private void FillRowEPADetailReport(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordEPADetailReport(memberEPA, phaseSelecteds, ref rowIndex);

            if ((rowIndex - 1) % 2 == 0)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(3, record);
            }
        }

        private void FillRowMonthlyEPADetailReport(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecordMonthlyEPADetailReport(memberEPA, phaseSelecteds, ref rowIndex);

            if ((rowIndex - 1) % 2 == 0)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(3, record);
            }
        }

        private object CreateRecordEPADetailReport(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref int rowIndex)
        {
            dynamic score = CalculateScore(memberEPA);

            var record = new
            {
                Count = rowIndex,
                Code = memberEPA.Employee.Code,
                FullName = memberEPA.Employee.FullName,
                NickName = memberEPA.Employee.NickName,
                Category = memberEPA.Employee.Category,
                Section = memberEPA.Employee.GroupName,
                JobTitle = memberEPA.Employee.JobTitleName,
                JoinedDate = memberEPA.Employee.JoinedDate.ToString(Constants.DateFormat),
                TargetType = memberEPA.Period.TargetTypeName,
                SelfScore = GetScoreByPhaseSelected(phaseSelecteds, score.SelfScore, (int)ProcessType.Self),
                EvaluatorScore = GetScoreByPhaseSelected(phaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator),
                ReviewerScore = GetScoreByPhaseSelected(phaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer),
                ManagementScore = GetScoreByPhaseSelected(phaseSelecteds, score.ManagementScore.ToString(), (int)ProcessType.Management),
                DisciplineScore = score.DisciplineScore,
                FinalScore = score.FinalScore,
                Grade = score.Grade
            };
            rowIndex++;

            return record;
        }

        private object CreateRecordMonthlyEPADetailReport(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref int rowIndex)
        {
            dynamic score = CalculateScore(memberEPA);

            var record = new
            {
                Count = rowIndex,
                Code = memberEPA.Employee.Code,
                FullName = memberEPA.Employee.FullName,
                NickName = memberEPA.Employee.NickName,
                Category = memberEPA.Employee.Category,
                Section = memberEPA.Employee.GroupName,
                JobTitle = memberEPA.Employee.JobTitleName,
                JoinedDate = memberEPA.Employee.JoinedDate.ToString(Constants.DateFormat),
                SelfScore = GetScoreByPhaseSelected(phaseSelecteds, score.SelfScore, (int)ProcessType.Self),
                EvaluatorScore = GetScoreByPhaseSelected(phaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator),
                ReviewerScore = GetScoreByPhaseSelected(phaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer),
                FinalScore = score.FinalScore,
                Grade = score.Grade
            };
            rowIndex++;

            return record;
        }

        private string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            var result = "-";

            if (phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1))
            {
                result = score;
            }

            return result;
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

            if (memberEPA.EPAId > 0)
            {
                float score = memberEPA.Process.Self.TotalScore.Length > 0 ? float.Parse(memberEPA.Process.Self.TotalScore) : 0;

                if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    score = memberEPA.MonthlyEPA.AverageSelfScoreConverted;
                }

                selfScore = Math.Round(score, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Evaluators != null && memberEPA.Process.Evaluators.Any())
            {
                evaluatorScore = Math.Round(memberEPA.Process.AverageScoreByEvaluators, 2).ToString();

                if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    evaluatorScore = Math.Round(memberEPA.MonthlyEPA.AverageEvaluatorScoreConverted, 2).ToString();
                }
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Reviewers != null && memberEPA.Process.Reviewers.Any())
            {
                reviewerScore = Math.Round(memberEPA.Process.AverageScoreByReviewers, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.Process.Managements != null && memberEPA.Process.Managements.Any())
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

        private void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }

        private MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary, string title)
        {
            IList<string> titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Category,
                Lang.Section,
                Lang.JobTitle,
                Lang.GroupDate
            };

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.EPASummaryReportSheet);

            IRow rowTitle = sheet.CreateRow(0);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(title);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, titleHeaders.Count + (epaSummary.PeriodSelecteds.Count * 2) - 1));

            RenderHeaderEPASummaryReport(epaSummary.PeriodSelecteds, ref workbook, ref sheet, titleHeaders);
            RenderContentRowEPASummaryReport(epaSummary, ref workbook, ref sheet);

            MemoryStream output = null;
            try
            {
                output = new MemoryStream();
                workbook.Write(output);

                return output;
            }
            finally
            {
                if (output != null)
                {
                    output.Dispose();
                }
            }
        }
    }
}