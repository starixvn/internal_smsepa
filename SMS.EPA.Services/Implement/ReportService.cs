// -----------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Fanex.Common.Helper;
    using NexFB.Core.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
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
        private const int _EXCEL_COLUMN_WIDTH = 256;
        private readonly IReportRepository _reportRepository;

        public ReportService()
            : this(null)
        {
        }

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public DashboardModel GetDashboard(GetDashboardCriteria criteria)
        {
            DashboardModel result = new DashboardModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _reportRepository.GetDashboard(criteria);
            }

            return result;
        }

        #region EPA Summary Report

        public EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            EPASummaryModel result = new EPASummaryModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _reportRepository.GetEPASummary(criteria);
            }

            return result;
        }

        public MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary)
        {
            IList<string> titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Category,
                Lang.Section,
                Lang.JobTitle
            };
            var titleString = Lang.EPASummaryReportTitle;

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.EPASummaryReportSheet);

            var rowTitle = sheet.CreateRow(0);

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            var cell = rowTitle.CreateCell(0);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, titleHeaders.Count + (epaSummary.PeriodSelecteds.Count * 2) - 1));

            RenderHeaderEPASummaryReport(epaSummary.PeriodSelecteds, ref workbook, ref sheet, titleHeaders);
            RenderContentRowEPASummaryReport(epaSummary, ref workbook, ref sheet);

            var output = new MemoryStream();
            workbook.Write(output);
            return output;
        }

        #endregion EPA Summary Report

        #region EPA Detail Report

        public EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria)
        {
            EPADetailModel result = new EPADetailModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _reportRepository.GetEPADetail(criteria);
            }

            return result;
        }

        public void ExportEPADetailReport(EPADetailModel epaDetail, string templatePath)
        {
            string reportName = Lang.EPADetailReportName;
            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.EPADetailReportSheet);

            writer.CopyRows(0, 3);

            FillContentRowEPADetailReport(epaDetail, ref writer);
            writer.SyncColumnsWidth(14);

            writer.Render();
        }

        #endregion EPA Detail Report

        private void RenderHeaderEPASummaryReport(
            IList<PeriodModel> periodSelecteds,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeaders)
        {
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = NPOI.HSSF.Util.HSSFColor.Grey80Percent.White.Index;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey80Percent.Index;
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
            bodyStyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.Left;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;
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
                        EPASummaryPeriodModel period = epaInfo.Periods.Where(x => x.PeriodId == epaSummary.PeriodSelecteds[i].PeriodId).FirstOrDefault();

                        if (period.IsEndPeriod.Equals(true))
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

                        cellBody = row.CreateCell(7 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(finalScore);
                        SetColumnWidth(ref sheet, 7 + (2 * i), 14);

                        cellBody = row.CreateCell(7 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(grade);
                        SetColumnWidth(ref sheet, 7 + ((2 * i) + 1), 10);
                    }
                    else
                    {
                        cellBody = row.CreateCell(7 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 7 + (2 * i), 14);

                        cellBody = row.CreateCell(7 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 7 + ((2 * i) + 1), 10);
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

            foreach (var memberEPA in epaDetail.MemberEPAs)
            {
                FillRowEPADetailReport(memberEPA, epaDetail.PhaseSelecteds, ref writer, ref rowIndex);
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
                Section = memberEPA.Employee.Group.GroupName,
                JobTitle = memberEPA.Employee.JobTitle.JobTitleName,
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

        private string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            var result = "-";

            if (phaseSelecteds.Where(x => x.PhaseId == phaseId).Count().Equals(1))
            {
                result = score;
            }

            return result;
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

            if (memberEPA.EPAId > 0 && memberEPA.ProcessBySelf.IsEndProcess)
            {
                float score = memberEPA.ProcessBySelf.TotalScore.Length > 0 ? float.Parse(memberEPA.ProcessBySelf.TotalScore) : 0;

                selfScore = Math.Round(score, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.ProcessByEvaluators != null && memberEPA.ProcessByEvaluators.Any()
                && memberEPA.ProcessByEvaluators.First().IsEndProcess)
            {
                evaluatorScore = Math.Round(memberEPA.AverageScoreByEvaluators, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.ProcessByReviewers != null && memberEPA.ProcessByReviewers.Any()
                && memberEPA.ProcessByReviewers.First().IsEndProcess)
            {
                reviewerScore = Math.Round(memberEPA.AverageScoreByReviewers, 2).ToString();
            }

            if (memberEPA.EPAId > 0 && memberEPA.ProcessByManagements != null && memberEPA.ProcessByManagements.Any()
                && memberEPA.ProcessByManagements.First().IsEndProcess)
            {
                managementScore = Math.Round(memberEPA.AverageScoreByManagements, 2).ToString();
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
            sheet.SetColumnWidth(columnIndex, width * _EXCEL_COLUMN_WIDTH);
        }
    }
}