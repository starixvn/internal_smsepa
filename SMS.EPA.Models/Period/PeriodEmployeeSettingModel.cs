namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Enitites;

    [Serializable]
    public class PeriodEmployeeSettingModel
    {
        public PeriodEmployeeSettingModel()
        {
            Group = new Group();
            HumanResources = new List<PeriodFlowModel>();
            SelfEvaluators = new List<PeriodFlowModel>();
            Evaluators = new List<PeriodFlowModel>();
            Reviewers = new List<PeriodFlowModel>();
            Managements = new List<PeriodFlowModel>();
        }

        public int PeriodId { get; set; }

        public string Name { get; set; }

        public Employee Employee { get; set; }

        public Department Department { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public IList<PeriodFlowModel> HumanResources { get; set; }

        public IList<PeriodFlowModel> SelfEvaluators { get; set; }

        public IList<PeriodFlowModel> Evaluators { get; set; }

        public IList<PeriodFlowModel> Reviewers { get; set; }

        public IList<PeriodFlowModel> Managements { get; set; }

        public bool IsIncluded { get; set; }

        public TargetTemplate TargetTemplate { get; set; }

        public int NumberEvaluators { get; set; }

        public bool IsSemiHasMonthly
        {
            get
            {
                return Employee != null && Employee.HasMonthlyEPA && TargetTemplate != null && TargetTemplate.TargetTypeId == (int)TargetTypes.SemiAnnual;
            }
        }
    }
}