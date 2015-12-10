// ----------------------------------------------------------------------
// <copyright file="UpdateFinalScoreEndEvaluationCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/01/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class UpdateFinalScoreEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        [SpParam(SqlParamName = "EPAScores")]
        public string EvaluationScores { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EvaluationScores);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateFinalScoreEndEvaluation";
        }

        public string ConvertToXml(int evaluationId, string finalScore, float disciplineScore, string grade)
        {
            var epaXmlBuilder = new StringBuilder(BeginRootTag);

            epaXmlBuilder.AppendFormat(TemplateXml, evaluationId, finalScore, disciplineScore, grade);
            epaXmlBuilder.Append(EndRootTag);

            return epaXmlBuilder.ToString();
        }
    }
}
