namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class AdjustScoreByReviewerOrManagementCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = @"<Row EPAId='{0}' ProcessId='{1}' EmployeeId='{2}'
            EvaluatorId='{3}' AdjustScore='{4}' TotalScore='{5}' Comment='{6}' />";

        private const string RecommendationXmlTemplate = @"<Row EPAId='{0}' ProbationStatus='{1}' Promoted='{2}' PayIncreased='{3}'
            JobGradeIncreased='{4}' JobGradeId='{5}' />";

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

        public string RecommendationXml
        {
            get
            {
                var result = new StringBuilder(Constants.BeginRootTag);
                if (Recommendations != null && Recommendations.Any())
                {
                    foreach (EPARecommendation recommendation in Recommendations)
                    {
                        int? jobGradeId =
                            (recommendation.JobGrade == null || !recommendation.JobGradeIncreased) ? (int?)null : recommendation.JobGrade.JobGradeId;

                        result.AppendFormat(
                                RecommendationXmlTemplate,
                                recommendation.EPAId,
                                recommendation.ProbationStatus,
                                recommendation.Promoted,
                                recommendation.PayIncreased,
                                recommendation.JobGradeIncreased,
                                jobGradeId);
                    }
                }

                result.Append(Constants.EndRootTag);

                return result.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<AdjustScoreViewModel> EpaInfos { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPARecommendation> Recommendations { get; set; }

        public byte ProcessId { get; set; }

        public override bool IsValid() => ModifiedBy > 0 && ProcessId > 0;

        public override string GetSettingKey() => "EPA_AdjustScoreByReviewerOrManagement";
    }
}