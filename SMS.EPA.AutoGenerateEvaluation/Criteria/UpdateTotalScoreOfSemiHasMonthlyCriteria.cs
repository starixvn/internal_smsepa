namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;

    public class UpdateTotalScoreOfSemiHasMonthlyCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}'></EPA>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int EPAId { get; set; }

        [SpParam(Ignored = true)]
        public MonthlyEPAModel MonthlyEPA { get; set; }

        public string Details
        {
            get
            {
                var epaXml = new StringBuilder(BeginRootTag);

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Self,
                        EncodeService.Encode(MonthlyEPA.AverageSelfScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Evaluator,
                        EncodeService.Encode(MonthlyEPA.AverageEvaluatorScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Reviewer,
                        EncodeService.Encode(MonthlyEPA.AverageReviewerScoreConverted.ToString()));

                epaXml.Append(EndRootTag);

                return epaXml.ToString();
            }
        }

        public override bool IsValid()
        {
            return EPAId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_SemiAnnual_UpdateTotalScore";
        }
    }
}