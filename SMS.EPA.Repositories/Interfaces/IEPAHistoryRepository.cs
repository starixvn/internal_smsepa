// ----------------------------------------------------------------------
// <copyright file="IEPAHistoryRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EPAHistory;

    public interface IEPAHistoryRepository
    {
        IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria);
    }
}
