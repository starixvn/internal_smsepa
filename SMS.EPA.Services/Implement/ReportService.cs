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
    using System.Collections.Generic;
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
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.EPASummaryReportSheet);
            var rowTitle = sheet.CreateRow(0);
            var titleString = Lang.EPASummaryReportTitle;

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            var cell = rowTitle.CreateCell(0);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            var titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Section,
                Lang.JobTitle
            };

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, epaSummary.PeriodSelecteds.Count + titleHeaders.Count - 1));

            RenderHeaderExportEPASummary(epaSummary.PeriodSelecteds, workbook, sheet, titleHeaders);
            RenderBodyExportEPASummary(epaSummary, workbook, sheet);

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

            FillContentRow(epaDetail, ref writer);
            writer.SyncColumnsWidth(13);

            writer.Render();
        }

        #endregion EPA Detail Report

        private void RenderHeaderExportEPASummary(IList<PeriodModel> periodSelecteds, HSSFWorkbook workbook, ISheet sheet, List<string> titleHeaders)
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

            var cellFinalScore = row.CreateCell(titleHeaders.Count);
            cellFinalScore.CellStyle = cellStyle;
            cellFinalScore.SetCellValue(Lang.FinalScore);

            if (periodSelecteds.Count > 0)
            {
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, titleHeaders.Count, titleHeaders.Count + periodSelecteds.Count - 1));
            }

            IRow rowPeriod = sheet.CreateRow(3);
            for (int i = 0; i < periodSelecteds.Count; i++)
            {
                var cell = rowPeriod.CreateCell(titleHeaders.Count + i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(periodSelecteds[i].Name);
            }
        }

        private void RenderBodyExportEPASummary(EPASummaryModel epaSummary, HSSFWorkbook workbook, ISheet sheet)
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

            foreach (var epaEmployee in epaSummary.EPAEmployees)
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

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaEmployee.Code);

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
                cellBody.SetCellValue(epaEmployee.FullName);

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
                cellBody.SetCellValue(epaEmployee.NickName);

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
                cellBody.SetCellValue(epaEmployee.Group.GroupName);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(epaEmployee.JobTitle.JobTitleName);

                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                List<int> periodIds = epaEmployee.Periods.Select(x => x.PeriodId).ToList();
                for (int i = 0; i < epaSummary.PeriodSelecteds.Count; i++)
                {
                    cellBody = row.CreateCell(6 + i);
                    cellBody.CellStyle = bodyStyle;

                    if (periodIds.Contains(epaSummary.PeriodSelecteds[i].PeriodId))
                    {
                        var finalScore = epaEmployee.Periods.Where(x => x.PeriodId == epaSummary.PeriodSelecteds[i].PeriodId).FirstOrDefault().FinalScore;
                        cellBody.SetCellValue(finalScore);
                    }
                    else
                    {
                        cellBody.SetCellValue("-");
                    }
                }

                rowIndex++;
            }
        }

        private void FillContentRow(EPADetailModel epaDetail, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (var memberEPA in epaDetail.MemberEPAs)
            {
                FillRow(memberEPA, epaDetail.PhaseSelecteds, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(memberEPA, phaseSelecteds, ref rowIndex);

            if (rowIndex % 2 == 0)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(3, record);
            }
        }

        private object CreateRecord(
            EPAModel memberEPA,
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = memberEPA.Employee.Code,
                FullName = memberEPA.Employee.FullName,
                NickName = memberEPA.Employee.NickName,
                Section = memberEPA.Employee.Group.GroupName,
                JobTitle = memberEPA.Employee.JobTitle.JobTitleName,
                TargetType = memberEPA.Period.TargetTypeName,
                SelfScore = GetScoreByPhaseSelected(phaseSelecteds, memberEPA.ProcessBySelf.TotalScore, (int)ProcessType.Self),
                EvaluatorScore = GetScoreByPhaseSelected(phaseSelecteds, memberEPA.AverageScoreByEvaluators, (int)ProcessType.Evaluator),
                ReviewerScore = GetScoreByPhaseSelected(phaseSelecteds, memberEPA.AverageScoreByReviewers, (int)ProcessType.Reviewer),
                ManagementScore = GetScoreByPhaseSelected(phaseSelecteds, memberEPA.AverageScoreByManagements, (int)ProcessType.Management),
                DisciplineScore = memberEPA.HasDisciplineScore ? memberEPA.DisciplineScore.ToString() : string.Empty,
                FinalScore = memberEPA.FinalScore,
                Grade = memberEPA.Grade
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
    }
}