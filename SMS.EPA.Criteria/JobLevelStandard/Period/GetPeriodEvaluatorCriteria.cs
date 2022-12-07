using Fanex.Data.Repository;
using Fanex.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.Criteria.JobLevelStandard.Period
{
    public class GetPeriodEvaluatorCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }
        public string TeamIds { get; set; }
        public bool? IsLocal { get; set; }
        public byte? StatusId { get; set; }
        public bool? IsExcluded { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_PeriodEmployeeSetting_GetEvaluators";

        public override bool IsValid() => PeriodId > 0;
    }
}
