// -----------------------------------------------------------------------
// <copyright file="IEmployeeEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;

    public interface IEmployeeEPARepository
    {
        RawEPAModel GetEPA(ICriteria criteria);

        RawEPAInfoModel GetEPAInfo(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria);
    }
}