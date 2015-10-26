// -----------------------------------------------------------------------
// <copyright file="GetPersonalFlowCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.26">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetPersonalFlowCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_Get";
        }
    }
}