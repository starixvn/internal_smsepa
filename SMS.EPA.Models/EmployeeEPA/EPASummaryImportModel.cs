// -----------------------------------------------------------------------
// <copyright file="EPASummaryImportModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.05">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Core.Services;

    public class EPASummaryImportModel
    {
        public int Id { get; set; }

        public float OriginFinalScore { get; set; }

        public string FinalScore
        {
            get
            {
                return EncodeService.Encode(OriginFinalScore.ToString());
            }
        }
    }
}
