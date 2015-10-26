// -----------------------------------------------------------------------
// <copyright file="GetDepartmentFlowByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetDepartmentFlowByIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int FlowTemplateId { get; set; }

        public bool IsGroup { get; set; }

        public override bool IsValid()
        {
            return DepartmentId > 0 && FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingDepartmentFlow_GetById";
        }
    }
}