// -----------------------------------------------------------------------
// <copyright file="EPAKPIGroupModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;

    public class EPAKPIGroupModel
    {
        public EPAKPIGroupModel()
        {
            KPIs = new List<EPAKPIModel>();
        }

        public int EPAId { get; set; }

        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public double? Weight { get; set; }

        public bool IsSpreadKPI { get; set; }

        public int DisplayOrder { get; set; }

        public bool HasKPIOptional
        {
            get
            {
                foreach (var kpi in KPIs)
                {
                    if (!kpi.IsRequiredEvaluate)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int TotalKPIOptional
        {
            get
            {
                var totalKPIOptional = 0;

                foreach (var kpi in KPIs)
                {
                    if (!kpi.IsRequiredEvaluate)
                    {
                        totalKPIOptional++;
                    }
                }

                return totalKPIOptional;
            }
        }

        public IList<EPAKPIModel> KPIs { get; set; }
    }
}