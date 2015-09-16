// ----------------------------------------------------------------------
// <copyright file="UpdateIgnoredViolationCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/16/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;

    public class UpdateIgnoredViolationCriteria : CriteriaBase
    {
        private const string _ViolationItem = "<Violation WorkDate='{0}' EmployeeId='{1}' ViolationId='{2}' Ignored='{3}'></Violation>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public string IgnoredViolations { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(IgnoredViolations);
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_IgnoredViolation";
        }

        public string ConvertViolationItemToXml(IEnumerable<IgnoredViolationModel> violations)
        {
            if (violations != null)
            {
                var value = new StringBuilder(_BeginRootTag);

                foreach (var item in violations)
                {
                    value.AppendFormat(_ViolationItem, item.WorkDate, item.EmployeeId, item.ViolationId, item.Ignored);
                }

                return value.Append(_EndRootTag).ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
