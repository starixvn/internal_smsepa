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
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.EmployeeEPA;

    public class AdjustScoreByReviewerOrManagementCriteria : CriteriaBase
    {
        private const string _EPA = "<EPA EPAId='{0}' ProcessId='{1}' EmployeeId='{2}' EvaluatorId='{3}' Score='{4}' Comment='{5}'></EPA>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public string EPADetails
        {
            get
            {
                var result = new StringBuilder(_BeginRootTag);
                var cipherScore = string.Empty;

                foreach (var memberEPA in MemberEPAs)
                {
                    if (ProcessId == (int)ProcessType.Reviewer)
                    {
                        cipherScore = !string.IsNullOrEmpty(memberEPA.ProcessByReviewers.FirstOrDefault().TotalScore) ? EncodeService.Encode(memberEPA.ProcessByReviewers.FirstOrDefault().TotalScore) : string.Empty;
                        result.AppendFormat(_EPA, memberEPA.EPAId, ProcessId, memberEPA.Employee.EmployeeId, ModifiedBy, cipherScore, HttpUtility.HtmlEncode(memberEPA.ProcessByReviewers.FirstOrDefault().Comment));
                    }
                    else if (ProcessId == (int)ProcessType.Management)
                    {
                        cipherScore = !string.IsNullOrEmpty(memberEPA.ProcessByManagements.FirstOrDefault().TotalScore) ? EncodeService.Encode(memberEPA.ProcessByManagements.FirstOrDefault().TotalScore) : string.Empty;
                        result.AppendFormat(_EPA, memberEPA.EPAId, ProcessId, memberEPA.Employee.EmployeeId, ModifiedBy, cipherScore, HttpUtility.HtmlEncode(memberEPA.ProcessByManagements.FirstOrDefault().Comment));
                    }
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
    }
}