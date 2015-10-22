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
                return ConvertToXml(EpaInfos);
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<RatingByKPIModel> EpaInfos { get; set; }

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

        private string ConvertToXml(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var result = new StringBuilder(_BeginRootTag);

            foreach (var epaInfo in epaInfos)
            {
                var detail = new StringBuilder();
                string rate = string.Empty,
                    cipherScore = string.Empty,
                    cipherRate = string.Empty,
                    totalScore = string.Empty;

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    if (!string.IsNullOrEmpty(kpi.AdjustScore) || kpi.AdjustRate > 0)
                    {
                        rate = kpi.AdjustRate > 0 ? kpi.AdjustRate.ToString() : string.Empty;
                        cipherScore = Encode(kpi.AdjustScore + "_" + kpi.KPIId);
                        cipherRate = Encode(rate);

                        detail.AppendFormat(_EPADetail, kpi.KPIId, cipherRate, cipherScore, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));
                    }
                    else if (!string.IsNullOrEmpty(kpi.Score) || kpi.Rate > 0)
                    {
                        rate = kpi.Rate > 0 ? kpi.Rate.ToString() : string.Empty;
                        cipherScore = Encode(kpi.Score + "_" + kpi.KPIId);
                        cipherRate = Encode(rate);

                        detail.AppendFormat(_EPADetail, kpi.KPIId, cipherRate, cipherScore, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));
                    }
                }

                totalScore = Encode(epaInfo.TotalScore);
                result.AppendFormat(_EPA, epaInfo.EPAId, epaInfo.Employee.EmployeeId, epaInfo.KPIGroup.KPIGroupId, totalScore, detail);
            }

            result.Append(_EndRootTag);

            return result.ToString();
        }
    }
}