// --------------------------------------------------------------------------
// <copyright file="GetEmployeeCalendarCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.10.2">Created</change>
// </history>

namespace NexFB.WebService.Criteria
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetTeam Criteria.
    /// </summary>
    public class GetEmployeeCalendarCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
