// ----------------------------------------------------------------------
// <copyright file="GetAllPeriodIncludeImportCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetAllPeriodIncludeImportCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetAllPeriodIncludeImport";
        }
    }
}
