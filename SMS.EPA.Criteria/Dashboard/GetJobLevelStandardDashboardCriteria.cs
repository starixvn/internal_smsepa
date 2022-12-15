using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.Criteria.Dashboard
{
    public class GetJobLevelStandardDashboardCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_GetDashboard";

        public override bool IsValid() => EmployeeId > 0;
    }
}
