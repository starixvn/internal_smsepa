// --------------------------------------------------------------------------
// <copyright file="GetEmployeeRequestHistoryCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.9.9">Created</change>
// </history>

namespace NexFB.WebService.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeRequestHistoryCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
