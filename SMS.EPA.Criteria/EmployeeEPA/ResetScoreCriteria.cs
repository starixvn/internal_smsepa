namespace SMS.EPA.Criteria
{
    using System.Text;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class ResetScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}' AdjustScore=''></EPA>";

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

                if (EPAInfo != null && EPAInfo.IsSemiHasMonthly)
                {
                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Self,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageSelfScoreConverted.ToString()));

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Evaluator,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString()));

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Reviewer,
                        cryptographyService.Encrypt(EPAInfo.MonthlyEPA.AverageReviewerScoreConverted.ToString()));

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Management,
                        string.Empty);
                }
                else
                {
                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Reviewer,
                        string.Empty);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (byte)ProcessType.Management,
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