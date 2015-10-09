﻿// -----------------------------------------------------------------------
// <copyright file="UpdateEPAScoreCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.13">Created</change>
// </history>

namespace SMS.EPA.Criteria.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.EmployeeEPA;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string _KPIScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' IsMaxScore='{4}' Comment='{5}'></KPI>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        public string Comment { get; set; }

        public string Score
        {
            get
            {
                var totalScore = EPAProcess.TotalScore;

                return !string.IsNullOrEmpty(totalScore) ? EncodeService.Encode(totalScore) : string.Empty;
            }
        }

        public string KPIDetails
        {
            get
            {
                var result = new StringBuilder(_BeginRootTag);

                foreach (var kpiGroup in KPIGroups)
                {
                    foreach (var kpi in kpiGroup.KPIs)
                    {
                        var score = kpi.CurrentEvaluation.Score;
                        var rate = kpi.CurrentEvaluation.Rate > 0 ? kpi.CurrentEvaluation.Rate.ToString() : string.Empty;
                        var isMaxScore = kpi.CurrentEvaluation.IsMaxScore;
                        var cipherScore = Encode(score + "_" + kpi.KPIId);
                        var cipherRate = Encode(rate);

                        result.AppendFormat(_KPIScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, cipherRate, isMaxScore, HttpUtility.HtmlEncode(kpi.CurrentEvaluation.Comment));
                    }
                }

                result.Append(_EndRootTag);

                return result.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAProcessModel EPAProcess { get; set; }

        [SpParam(Ignored = true)]
        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EmployeeId > 0 && EvaluatorId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_UpdateScore";
        }

        private string Encode(string plainText)
        {
            return !string.IsNullOrEmpty(plainText) ? EncodeService.Encode(plainText) : string.Empty;
        }
    }
}