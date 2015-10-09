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
        private string template = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";
        private string beginRootTag = "<ROOT>";
        private string endRootTag = "</ROOT>";

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
            var value = new StringBuilder(beginRootTag);

            value.AppendFormat(template, evaluationId, finalScore, disciplineScore, grade);
            value.Append(endRootTag);

            return value.ToString();
        }
    }
}
