namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Period;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";

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
            var epaProcessXmlBuilder = new StringBuilder(Constants.BeginRootTag);

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                epaProcessXmlBuilder.AppendFormat(ProcessXmlTemplate, evaluationProcess.EPAId, evaluationProcess.ProcessId);
            }

            return epaProcessXmlBuilder.Append(Constants.EndRootTag).ToString();
        }
    }
}