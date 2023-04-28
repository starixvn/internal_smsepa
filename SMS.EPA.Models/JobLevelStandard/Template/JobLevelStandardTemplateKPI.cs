using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.Model.JobLevelStandard.Template
{
    public class JobLevelStandardTemplateKPI
    {
        public int KPIId { get; set; }

        public string Name { get; set; }

        public int? DisplayOrder { get; set; }
        public bool IsSelected { get; set; }
        public double? Weight { get; set; }

    }
}
