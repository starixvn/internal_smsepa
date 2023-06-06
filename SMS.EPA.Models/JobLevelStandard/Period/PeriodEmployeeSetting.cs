namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model.Report;

    [Serializable]
    public class PeriodEmployeeSetting : IExcelRecord
    {
        public int PeriodId { get; set; }
        public int? AssessmentId { get; set; }
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string JobLevel { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }
        public string Template { get; set; }
        public int TemplateId { get; set; }
        public bool IsExcluded { get; set; }

        public JobLevelAssessmentStatus? StatusId { get; set; }

        public IList<PeriodProcessSetting> Evaluators { get; set; } = new List<PeriodProcessSetting>();
        public PeriodProcessSetting Employee { get; set; }
        public IList<PeriodProcessSetting> Managements { get; set; } = new List<PeriodProcessSetting>();

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Department,
                Section,
                Team,
                FullName,
                Nickname,
                JobLevel,
                Evaluator = GetEvaluator(Evaluators),
                Management = GetEvaluator(Managements),
                Status = Enumerations.GetDescription(StatusId),
            };

            return record;
        }

        private static string GetEvaluator(IList<PeriodProcessSetting> evaluators)
        {
            var nicknames = evaluators
                .Where(x => !string.IsNullOrEmpty(x.FullName) || !string.IsNullOrEmpty(x.Nickname))
                .Select(x => string.Format(CultureInfo.InvariantCulture, "- {0} ({1})", x.FullName, x.Nickname)).ToList();

            return string.Join(Environment.NewLine, nicknames);
        }
    }
}