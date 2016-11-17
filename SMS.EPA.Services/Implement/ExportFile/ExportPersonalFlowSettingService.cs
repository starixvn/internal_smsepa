namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class ExportPersonalFlowSettingService : IExportPersonalFlowSettingService
    {
        private const string TemplatePath = @"~\ExcelTemplates\PersonalFlowSettingTemplate.xls";
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 9;

        private readonly IPersonalFlowService personalFlowService;

        public ExportPersonalFlowSettingService()
            : this(null)
        {
        }

        public ExportPersonalFlowSettingService(IPersonalFlowService personalFlowService)
        {
            this.personalFlowService = personalFlowService ?? new PersonalFlowService();
        }

        public void Export(int flowTemplateId, string flowTemplateName)
        {
            IEnumerable<PersonalFlowModel> flowSettings = personalFlowService.GetPersonalFlowByFlowTemplateId(flowTemplateId);

            if (flowSettings.Any())
            {
                string reportTitle = string.Format("{0} - Personal Flow Setting", flowTemplateName),
                    templatePath = HttpContext.Current.Server.MapPath(TemplatePath);

                var writer = new ExcelTemplateWriter(templatePath, "Personal_Flow_Setting", "Personal Flow");

                // Fill report title.
                writer.WriteRow(0, new object[] { reportTitle });
                writer.CopyRows(1, 1);

                // Fill report header.
                writer.CopyRows(RowStartHeader, 1);

                FillContentRow(flowSettings, ref writer);
                writer.SyncColumnsWidth(TotalColumns);
                writer.CreateFreezePane(0, RowStartBody);

                writer.Render();
            }
        }

        private void FillContentRow(
         IEnumerable<PersonalFlowModel> flowSettings,
         ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (PersonalFlowModel flowSetting in flowSettings)
            {
                FillRow(flowSetting, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            PersonalFlowModel flowSetting,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(flowSetting, ref rowIndex);
            writer.WriteRow(RowStartBody, record);
        }

        private object CreateRecord(
            PersonalFlowModel flowSetting,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = flowSetting.Code,
                FullName = flowSetting.FullName,
                NickName = flowSetting.NickName,
                Section = flowSetting.Group != null ? flowSetting.Group.GroupName : string.Empty,
                HumanResource = GetEvaluator(flowSetting.HumanResources),
                Evaluator = GetEvaluator(flowSetting.Evaluators),
                Reviewer = GetEvaluator(flowSetting.Reviewers),
                Management = GetEvaluator(flowSetting.Managements)
            };
            rowIndex++;

            return record;
        }

        private object GetEvaluator(IList<ProcessFlowModel> evaluators)
        {
            IEnumerable<string> nickNames = evaluators.Select(x => x.FullName + (!string.IsNullOrEmpty(x.NickName) ? " (" + x.NickName + ")" : string.Empty));
            string evaluatorNames = string.Join(", ", nickNames);

            return evaluatorNames;
        }
    }
}