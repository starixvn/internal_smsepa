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
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        private const string _EPA = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
        private const string _EPADetail = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' IsMaxScore='{3}' Comment='{4}' />";
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
                    var totalScore = string.Empty;

                    foreach (TemporaryKPIModel kpi in model.KPIGroup.KPIs)
                    {
                        if (!string.IsNullOrEmpty(kpi.AdjustScore) || kpi.AdjustRate > 0)
                        {
                            var score = kpi.AdjustScore;
                            var rate = kpi.AdjustRate > 0 ? kpi.AdjustRate.ToString() : string.Empty;
                            var isMaxScore = kpi.IsMaxScore;
                            var cipherScore = Encode(score + "_" + kpi.KPIId);
                            var cipherRate = Encode(rate);

                            detail.AppendFormat(_EPADetail, kpi.KPIId, cipherRate, cipherScore, isMaxScore, HttpUtility.HtmlEncode(kpi.Comment));
                        }
                        else if (!string.IsNullOrEmpty(kpi.ScoreEncode) || !string.IsNullOrEmpty(kpi.RateEncode))
                        {
                            detail.AppendFormat(_EPADetail, kpi.KPIId, kpi.RateEncode, kpi.ScoreEncode, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));
                        }
                    }

                    totalScore = Encode(model.TotalScore);
                    result.AppendFormat(_EPA, model.EPAId, model.Employee.EmployeeId, model.KPIGroup.KPIGroupId, totalScore, detail);
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

        private string Encode(string plainText)
        {
            return !string.IsNullOrEmpty(plainText) ? EncodeService.Encode(plainText) : string.Empty;
        }
    }
}