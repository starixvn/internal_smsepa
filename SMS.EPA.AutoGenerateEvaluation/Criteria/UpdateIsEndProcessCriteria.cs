// ----------------------------------------------------------------------
// <copyright file="UpdateIsEndProcessCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Period;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private string processTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";
        private string beginRootTag = "<ROOT>";
        private string endRootTag = "</ROOT>";

        public string EPAProcess { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPAProcess);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateIsEndProcess";
        }

        public string ConvertEvaluationProcessToXml(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var value = new StringBuilder(beginRootTag);

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                value.AppendFormat(processTemplate, evaluationProcess.EPAId, evaluationProcess.ProcessId);
            }

            value.Append(endRootTag);
            return value.ToString();
        }
    }
}
