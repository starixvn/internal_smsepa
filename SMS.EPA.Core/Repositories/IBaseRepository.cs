// ----------------------------------------------------------------------
// <copyright file="IBaseRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:28:33 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Repositories
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;

    /// <summary>
    /// The interface of base repository.
    /// </summary>
    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}
