namespace SMS.EPA.Criteria.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;

    public class GetDataToUpdateTotalScoreOfProcessCriteria : CriteriaBase
    {
        public string EmployeeIds { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EPA_GetDataToUpdateTotalScoreOfProcess";
    }
}