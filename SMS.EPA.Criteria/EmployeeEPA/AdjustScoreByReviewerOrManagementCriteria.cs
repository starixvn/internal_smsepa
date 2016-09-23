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
                var epaXml = new StringBuilder(Constants.BeginRootTag);

                foreach (EPAModel memberEPA in MemberEPAs)
                {
                    var cipherTotalScore = EncodeService.Encode(memberEPA.Process.CurrentProcess.TotalScore);
                    var cipherAdjustScore = EncodeService.Encode(memberEPA.Process.CurrentProcess.AdjustScore);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        memberEPA.EPAId,
                        ProcessId,
                        memberEPA.Employee.EmployeeId,
                        ModifiedBy,
                        cipherAdjustScore,
                        cipherTotalScore,
                        HttpUtility.HtmlEncode(memberEPA.Process.CurrentProcess.Comment));
                }

                epaXml.Append(Constants.EndRootTag);

                return epaXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

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