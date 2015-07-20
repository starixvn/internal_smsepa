// -----------------------------------------------------------------------
// <copyright file="CreateGroupFlowCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.20">Created</change>
// </history>

namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class CreateGroupFlowCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string FlowDetails { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return GroupId != 0;
        }

        public override string GetSettingKey()
        {
            return "SettingFlowGroup_Set";
        }
    }
}