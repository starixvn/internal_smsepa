// -----------------------------------------------------------------------
// <copyright file="FlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.22">Created</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDepartmentIdCriteria : CriteriaBase
    {
        public int? DepartmentId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByDepartmentId";
        }
    }
}