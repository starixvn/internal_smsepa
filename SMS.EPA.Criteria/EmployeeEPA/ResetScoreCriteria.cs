namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Text;

    public class ResetScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}' AdjustScore='{2}'></EPA>";

        public int EPAId { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAModel EPAInfo { get; set; }

        public string Details
        {
            get
            {
                ICryptographyService cryptographyService = new CryptographyService();
                var epaXml = new StringBuilder(Constants.BeginRootTag);

                if (EPAInfo.IsSemiHasMonthly)
                {
                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Self,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageSelfScoreConverted.ToString()),
                        string.Empty);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Evaluator,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString()),
                        string.Empty);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Reviewer,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageReviewerScoreConverted.ToString()),
                        string.Empty);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Management,
                        string.Empty,
                        string.Empty);
                }
                else
                {
                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Reviewer,
                        string.Empty,
                        string.Empty);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Management,
                        string.Empty,
                        string.Empty);
                }

                epaXml.Append(Constants.EndRootTag);

                return epaXml.ToString();
            }
        }

        public override bool IsValid()
        {
            return EPAId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_ResetScore";
        }
    }
}