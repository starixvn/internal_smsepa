// -----------------------------------------------------------------------
// <copyright file="AdjustScoreByReviewerOrManagementCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.21">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.EmployeeEPA;

    public class AdjustScoreByReviewerOrManagementCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = @"<EPA EPAId='{0}' ProcessId='{1}' EmployeeId='{2}'
            EvaluatorId='{3}' AdjustScore='{4}' TotalScore='{5}' Comment='{6}'></EPA>";

        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public string EPADetails
        {
            get
            {
                var epaXml = new StringBuilder(BeginRootTag);

                foreach (EPAModel memberEPA in MemberEPAs)
                {
                    var cipherTotalScore = EncodeService.Encode(memberEPA.CurrentProcess.TotalScore);
                    var cipherAdjustScore = EncodeService.Encode(memberEPA.CurrentProcess.AdjustScore);

                    epaXml.AppendFormat(
                        EPAXmlTemplate,
                        memberEPA.EPAId,
                        ProcessId,
                        memberEPA.Employee.EmployeeId,
                        ModifiedBy,
                        cipherAdjustScore,
                        cipherTotalScore,
                        HttpUtility.HtmlEncode(memberEPA.CurrentProcess.Comment));
                }

                epaXml.Append(EndRootTag);

                return epaXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        [SpParam(Ignored = true)]
        public int ProcessId { get; set; }

        public override bool IsValid()
        {
            return ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_AdjustScoreByReviewerOrManagement";
        }
    }
}