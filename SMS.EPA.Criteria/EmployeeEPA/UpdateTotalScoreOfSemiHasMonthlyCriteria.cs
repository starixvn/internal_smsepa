namespace SMS.EPA.Criteria.EmployeeEPA
{
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;

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
                        (byte)ProcessType.Self,
                        cryptographyService.Encrypt(MonthlyEPA.AverageSelfScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Evaluator,
                        cryptographyService.Encrypt(MonthlyEPA.AverageEvaluatorScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Reviewer,
                        cryptographyService.Encrypt(MonthlyEPA.AverageReviewerScoreConverted.ToString()));

                epaXml.Append(Constants.EndRootTag);

                return epaXml.ToString();
            }
        }

        public override bool IsValid() => EPAId > 0;

        public override string GetSettingKey() => "EpaAuto_SemiAnnual_UpdateTotalScore";
    }
}