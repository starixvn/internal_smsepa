namespace SMS.EPA.AutoService.Criteria
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Text;

    public class UpdateTotalScoreOfSemiHasMonthlyCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}'></EPA>";

        public int EPAId { get; set; }

        [SpParam(Ignored = true)]
        public MonthlyEPAModel MonthlyEPA { get; set; }

        public string Details
        {
            get
            {
                ICryptographyService cryptographyService = new CryptographyService();
                var epaXml = new StringBuilder(Constants.BeginRootTag);

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Self,
                        cryptographyService.Encrypt(MonthlyEPA.AverageSelfScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Evaluator,
                        cryptographyService.Encrypt(MonthlyEPA.AverageEvaluatorScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Reviewer,
                        cryptographyService.Encrypt(MonthlyEPA.AverageReviewerScoreConverted.ToString()));

                epaXml.Append(Constants.EndRootTag);

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