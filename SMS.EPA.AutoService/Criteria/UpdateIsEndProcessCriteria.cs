namespace SMS.EPA.AutoService.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Period;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public string EPAProcess
        {
            get
            {
                return ConvertEvaluationProcessToXml(EvaluationProcesses);
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<AutoEmployeePeriodModel> EvaluationProcesses { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPAProcess);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateIsEndProcess";
        }

        private string ConvertEvaluationProcessToXml(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var epaProcessXmlBuilder = new StringBuilder(BeginRootTag);

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                epaProcessXmlBuilder.AppendFormat(ProcessXmlTemplate, evaluationProcess.EPAId, evaluationProcess.ProcessId);
            }

            epaProcessXmlBuilder.Append(EndRootTag);

            return epaProcessXmlBuilder.ToString();
        }
    }
}