namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class ExportEmployeeFlowSettingService : IExportEmployeeFlowSettingService
    {
        private const string TemplatePath = @"~\ExcelTemplates\EmployeeFlowSettingTemplate.xls";

        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 13;

        public void Export(
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
            string periodName)
        {
            string fileName = string.Format(Lang.EmployeeFlowSettingReportName, periodName);
            fileName = fileName.Replace(" ", "_");
            string reportTitle = string.Format(Lang.EmployeeFlowSettingReportTitle, periodName),
                templatePath = HttpContext.Current.Server.MapPath(TemplatePath);

            var writer = new ExcelTemplateWriter(templatePath, fileName, Lang.EmployeeFlowSettingReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRow(employeeFlowSettings, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRow(
         IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
         ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeFlowSettings)
            {
                FillRow(employeeSetting, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            PeriodEmployeeSettingModel employeeSetting,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(employeeSetting, ref rowIndex);
            writer.WriteRow(RowStartBody, record);
        }

        private object CreateRecord(
            PeriodEmployeeSettingModel employeeSetting,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Department = employeeSetting.Employee.Department != null ? employeeSetting.Employee.Department.Name : string.Empty,
                Section = employeeSetting.Employee.Group != null ? employeeSetting.Employee.Group.GroupName : string.Empty,
                Code = employeeSetting.Employee.Code,
                FullName = employeeSetting.Employee.FullName,
                NickName = employeeSetting.Employee.NickName,
                JobTitle = employeeSetting.Employee.JobTitle.JobTitleName,
                JoinedDate = employeeSetting.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = employeeSetting.Employee.Category,
                Evaluator = GetEvaluator(employeeSetting.Evaluators),
                Reviewer = GetEvaluator(employeeSetting.Reviewers),
                Management = GetEvaluator(employeeSetting.Managements),
                Form = employeeSetting.TargetTemplate != null ? employeeSetting.TargetTemplate.TargetTypeName : string.Empty
            };
            rowIndex++;

            return record;
        }

        private object GetEvaluator(IList<PeriodFlowModel> evaluators)
        {
            IEnumerable<string> nickNames = evaluators.Where(x => !string.IsNullOrEmpty(x.EvaluatorInfo.NickName)).Select(x => x.EvaluatorInfo.NickName);
            string evaluatorNames = string.Join(", ", nickNames);

            return evaluatorNames;
        }
    }
}