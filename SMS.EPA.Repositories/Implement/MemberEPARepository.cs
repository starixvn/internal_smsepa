// -----------------------------------------------------------------------
// <copyright file="MemberEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.MemberEPA;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Interfaces;

    public class MemberEPARepository : IMemberEPARepository
    {
        public IEnumerable<MemberEPAModel> GetEPAByEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MemberEPAModel, TargetTemplateModel, EmployeeModel, MemberEPAModel>(
                    (me, ta, ev) =>
                    {
                        me.Target = ta;
                        me.Evaluator = ev;
                        return me;
                    },
                    "TargetTemplateId,EvaluatorId",
                    criteria.GetSpParams());
            }
        }
    }
}