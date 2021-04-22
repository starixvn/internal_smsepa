namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string KPIScoreXmlTemplate = "<Row KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' IsMaxScore='{4}' Comment='{5}' />";
        private const string ExtendedQuestionXMLTemplate = "<Row SubQuestionId='{0}' IsAgree='{1}' />";
        private const string RecommendationXmlTemplate = @"<Row EPAId='{0}' ProbationStatus='{1}' Promoted='{2}' PayIncreased='{3}'
            JobGradeIncreased='{4}' JobGradeId='{5}' />";

        private readonly ICryptographyService _cryptographyService;

        public UpdateEPAScoreCriteria()
        {
            _cryptographyService = new CryptographyService();
            ProbationSubQuestions = new List<ProbationSubQuestionModel>();
        }

        public int EPAId { get; set; }
        public byte ProcessId { get; set; }
        public int EmployeeId { get; set; }
        public int EvaluatorId { get; set; }

        public string Comment => EPAProcess.Comment;

        public string Score
        {
            get
            {
                string totalScore = EPAProcess.TotalScore;
                return _cryptographyService.Encrypt(totalScore);
            }
        }

        public string KPIDetails
        {
            get
            {
                var kpiDetailXml = new StringBuilder(Constants.BeginRootTag);
                foreach (EPAKpiGroupModel kpiGroup in KPIGroups)
                {
                    foreach (EPAKpiModel kpi in kpiGroup.KPIs)
                    {
                        string score = kpi.CurrentEvaluation.Score;
                        string rate = kpi.CurrentEvaluation.Rate > 0 ? kpi.CurrentEvaluation.Rate.ToString(CultureInfo.InvariantCulture) : string.Empty;
                        string cipherScore = _cryptographyService.Encrypt(score + "_" + kpi.KPIId);
                        string cipherRate = _cryptographyService.Encrypt(rate);
                        bool isMaxScore = kpi.CurrentEvaluation.IsMaxScore;

                        kpiDetailXml.AppendFormat(
                            KPIScoreXmlTemplate,
                            kpi.KPIId,
                            kpiGroup.KPIGroupId,
                            cipherScore,
                            cipherRate,
                            isMaxScore,
                            HttpUtility.HtmlEncode(kpi.CurrentEvaluation.Comment));
                    }
                }

                kpiDetailXml.Append(Constants.EndRootTag);
                return kpiDetailXml.ToString();
            }
        }

        public string ExtendedQuestions
        {
            get
            {
                var extendedQuestionXML = new StringBuilder();
                if (ProbationSubQuestions != null && ProbationSubQuestions.Any())
                {
                    extendedQuestionXML.Append(Constants.BeginRootTag);
                    foreach (var question in ProbationSubQuestions)
                    {
                        extendedQuestionXML.AppendFormat(ExtendedQuestionXMLTemplate, question.SubQuestionId, question.IsAgree);
                    }

                    extendedQuestionXML.Append(Constants.EndRootTag);
                }

                return extendedQuestionXML.ToString();
            }
        }

        public string RecommendationXml
        {
            get
            {
                var result = new StringBuilder();
                if (Recommendations != null && Recommendations.Any())
                {
                    result.Append(Constants.BeginRootTag);
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

                    result.Append(Constants.EndRootTag);
                }

                return result.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAProcessModel EPAProcess { get; set; }

        [SpParam(Ignored = true)]
        public IList<EPAKpiGroupModel> KPIGroups { get; set; }

        [SpParam(Ignored = true)]
        public IList<ProbationSubQuestionModel> ProbationSubQuestions { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPARecommendation> Recommendations { get; set; }

        public override bool IsValid() => EPAId > 0 && EmployeeId > 0 && EvaluatorId > 0 && ProcessId > 0;

        public override string GetSettingKey() => "EPA_UpdateScore";
    }
}