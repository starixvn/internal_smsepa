using SMS.EPA.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.Model.Dashboard
{
    public class JobLevelStandardDashboard
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public int FinishedByEvaluator { get; set; }

        public int UnfinishedByEvaluator { get; set; }

        public int FinishedByManagement { get; set; }

        public int UnfinishedByManagement { get; set; }
    }
}
