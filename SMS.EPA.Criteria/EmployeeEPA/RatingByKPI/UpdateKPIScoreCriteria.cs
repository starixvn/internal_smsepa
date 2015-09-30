// -----------------------------------------------------------------------
// <copyright file="UpdateKPIScoreCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.10">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        private const string _EPA = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
        private const string _EPADetail = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' Comment='{3}' />";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public int EvaluatorId { get; set; }

        public string KPIDetails
        {
            get
            {
                var result = new StringBuilder(_BeginRootTag);

                foreach (var model in RatingByKPIs)
                {
                    var detail = new StringBuilder();

                    foreach (var kpi in model.KPIGroup.KPIs)
                    {
                        if (kpi.Score != kpi.AdjustScore && kpi.AdjustRate > 0)
                        {
                            detail.AppendFormat(_EPADetail, kpi.KPIId, kpi.AdjustRateEncode, kpi.AdjustScoreEncode, HttpUtility.HtmlEncode(kpi.Comment));
                        }
                        else
                        {
                            detail.AppendFormat(_EPADetail, kpi.KPIId, kpi.RateEncode, kpi.ScoreEncode, HttpUtility.HtmlEncode(kpi.Comment));
                        }
                    }

                    result.AppendFormat(_EPA, model.EPAId, model.Employee.EmployeeId, model.KPIGroup.KPIGroupId, model.TotalScoreEncode, detail);
                }

                result.Append(_EndRootTag);

                return result.ToString();
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<RatingByKPIModel> RatingByKPIs { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_UpdateKPIScore";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}