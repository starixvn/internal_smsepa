// -----------------------------------------------------------------------
// <copyright file="UpdateEPASummaryImportCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.05">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EmployeeEPA;

    public class UpdateEPASummaryImportCriteria : CriteriaBase
    {
        private const string _EPA = "<EPA Id='{0}' SelfScore='{1}' EvaluatorScore='{2}' ReviewerScore='{3}' FinalScore='{4}' FinalGrade='{5}'></EPA>";

        [SpParam(Ignored = true)]
        public IEnumerable<EPASummaryImportModel> MemberEPAs { get; set; }

        public string EPADetailXML
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (MemberEPAs != null)
                {
                    foreach (EPASummaryImportModel epa in MemberEPAs)
                    {
                        value.AppendFormat(
                            _EPA,
                            epa.Id,
                            epa.SelfScore,
                            epa.EvaluatorScore,
                            epa.ReviewerScore,
                            epa.FinalScore,
                            epa.FinalGrade);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPADetailXML);
        }

        public override string GetSettingKey()
        {
            return "EPASummaryImport_Update";
        }
    }
}