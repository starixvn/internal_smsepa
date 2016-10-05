namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;

    public class AdjustScoreByReviewerOrManagementCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = @"<EPA EPAId='{0}' ProcessId='{1}' EmployeeId='{2}'
            EvaluatorId='{3}' AdjustScore='{4}' TotalScore='{5}' Comment='{6}'></EPA>";

        public string EPADetails
        {
            get
            {
                ICryptographyService cryptographyService = new CryptographyService();
                var epaXml = new StringBuilder(Constants.BeginRootTag);

                foreach (AdjustScoreViewModel epaInfo in EpaInfos)
                {
                    var totalScoreEncrypt = cryptographyService.Encrypt(epaInfo.TotalScore);
                    var adjustScoreEncrypt = cryptographyService.Encrypt(epaInfo.AdjustScore);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        epaInfo.EPAId,
                        ProcessId,
                        epaInfo.EmployeeId,
                        ModifiedBy,
                        adjustScoreEncrypt,
                        totalScoreEncrypt,
                        HttpUtility.HtmlEncode(epaInfo.Comment));
                }

                epaXml.Append(Constants.EndRootTag);

                return epaXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<AdjustScoreViewModel> EpaInfos { get; set; }

        public int ProcessId { get; set; }

        public override bool IsValid()
        {
            return ModifiedBy > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_AdjustScoreByReviewerOrManagement";
        }
    }
}