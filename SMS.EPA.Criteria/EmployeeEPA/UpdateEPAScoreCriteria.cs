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
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.EmployeeEPA;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string _KPIScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' Comment='{4}'></KPI>";
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
                return EPAProcess.TotalScoreEncode;
            }
        }

        public string KPIDetails
        {
            get
            {
                var result = new StringBuilder(_BeginRootTag);
                var cipherScore = string.Empty;

                foreach (var kpiGroup in KPIGroups)
                {
                    foreach (var kpi in kpiGroup.KPIs)
                    {
                        if (ProcessId == (int)ProcessType.Self)
                        {
                            cipherScore = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? kpi.EvaluateBySelf.ScoreEncode : string.Empty;
                            result.AppendFormat(_KPIScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateBySelf.Rate, HttpUtility.HtmlEncode(kpi.EvaluateBySelf.Comment));
                        }
                        else if (ProcessId == (int)ProcessType.Evaluator)
                        {
                            cipherScore = !string.IsNullOrEmpty(kpi.EvaluateByEvaluators.FirstOrDefault().Score) ? kpi.EvaluateByEvaluators.FirstOrDefault().ScoreEncode : string.Empty;
                            result.AppendFormat(_KPIScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateByEvaluators.FirstOrDefault().Rate, HttpUtility.HtmlEncode(kpi.EvaluateByEvaluators.FirstOrDefault().Comment));
                        }
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
    }
}