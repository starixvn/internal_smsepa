// -----------------------------------------------------------------------
// <copyright file="UpdateStatusEPACriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.19">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class UpdateStatusEPACriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public int ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int Status { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_UpdateStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}