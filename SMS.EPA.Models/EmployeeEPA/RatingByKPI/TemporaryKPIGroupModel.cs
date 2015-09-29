// -----------------------------------------------------------------------
// <copyright file="TemporaryKPIGroupModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.09">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA.RatingByKPI
{
    using System.Collections.Generic;

    public class TemporaryKPIGroupModel
    {
        public TemporaryKPIGroupModel()
        {
            KPIs = new List<TemporaryKPIModel>();
        }

        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public double? Weight { get; set; }

        public bool IsSpreadKPI { get; set; }

        public int NumberKPI { get; set; }

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

        public IList<TemporaryKPIModel> KPIs { get; set; }
    }
}