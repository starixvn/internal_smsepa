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
        private const string _EPA = "<EPA EPAId='{0}' ProcessId='{1}' EmployeeId='{2}' EvaluatorId='{3}' AdjustScore='{4}' TotalScore='{5}' Comment='{6}'></EPA>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public string EPADetails
        {
            get
            {
                var result = new StringBuilder(_BeginRootTag);

                foreach (var memberEPA in MemberEPAs)
                {
                    var cipherTotalScore = Encode(memberEPA.CurrentProcess.TotalScore);
                    var cipherAdjustScore = Encode(memberEPA.CurrentProcess.AdjustScore);

                    result.AppendFormat(_EPA, memberEPA.EPAId, ProcessId, memberEPA.Employee.EmployeeId, ModifiedBy, cipherAdjustScore, cipherTotalScore, HttpUtility.HtmlEncode(memberEPA.CurrentProcess.Comment));
                }

                result.Append(_EndRootTag);

                return result.ToString();
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

        private string Encode(string plainText)
        {
            return !string.IsNullOrEmpty(plainText) ? EncodeService.Encode(plainText) : string.Empty;
        }
    }
}