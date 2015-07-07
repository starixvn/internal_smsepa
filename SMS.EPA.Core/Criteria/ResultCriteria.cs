// ----------------------------------------------------------------------
// <copyright file="ResultCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:34:47 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// The result criteria.
    /// </summary>
    public class ResultCriteria : CriteriaBase
    {
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}
