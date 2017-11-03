namespace SMS.EPA.AutoService.Criteria
{
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateFinalScoreEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";

        [SpParam(SqlParamName = "EPAScores")]
        public string EvaluationScores => ConvertToXml();

        [SpParam(Ignored = true)]
        public int EPAId { get; set; }

        [SpParam(Ignored = true)]
        public string FinalScore { get; set; }

        [SpParam(Ignored = true)]
        public string Grade { get; set; }

        [SpParam(Ignored = true)]
        public float DisciplineScore { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && !string.IsNullOrWhiteSpace(EvaluationScores);
        }

        public override string GetSettingKey() => "EpaAuto_UpdateFinalScoreEndEvaluation";

        private string ConvertToXml()
        {
            var epaXmlBuilder = new StringBuilder(Constants.BeginRootTag);

            epaXmlBuilder.AppendFormat(TemplateXml, EPAId, FinalScore, DisciplineScore, Grade);
            epaXmlBuilder.Append(Constants.EndRootTag);

            return epaXmlBuilder.ToString();
        }
    }
}