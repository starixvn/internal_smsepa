// -----------------------------------------------------------------------
// <copyright file="IDepartmentService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.21">Created</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Department;

    public interface IDepartmentService
    {
        List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria);
    }
}