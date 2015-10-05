// -----------------------------------------------------------------------
// <copyright file="GetEPASummaryImportCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.05">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPASummaryImportCriteria : CriteriaBase
    {
        public override bool IsValid() 
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EPASummaryImport_Get";
        }
    }
}