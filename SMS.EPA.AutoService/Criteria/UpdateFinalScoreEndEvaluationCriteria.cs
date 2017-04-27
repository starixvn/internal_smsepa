namespace SMS.EPA.AutoService.Criteria
{
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Text;

    public class UpdateFinalScoreEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";

        [SpParam(SqlParamName = "EPAScores")]
        public string EvaluationScores { get; set; }

        public static string ConvertToXml(int evaluationId, string finalScore, float disciplineScore, string grade)
        {
            var epaXmlBuilder = new StringBuilder(Constants.BeginRootTag);

            epaXmlBuilder.AppendFormat(TemplateXml, evaluationId, finalScore, disciplineScore, grade);
            epaXmlBuilder.Append(Constants.EndRootTag);

            return epaXmlBuilder.ToString();
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EvaluationScores);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateFinalScoreEndEvaluation";
        }
    }
}