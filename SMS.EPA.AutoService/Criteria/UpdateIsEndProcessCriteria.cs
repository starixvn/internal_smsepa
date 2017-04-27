namespace SMS.EPA.AutoService.Criteria
{
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Text;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";

        public string EPAProcess => ConvertEvaluationProcessToXml(EvaluationProcesses);

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

        private static string ConvertEvaluationProcessToXml(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
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