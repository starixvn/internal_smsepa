namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class PeriodFlowModel : FlowModel
    {
        public int EmployeeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool HasEvaluationGenerated { get; set; }

        public int ProcessStatus { get; set; }

        public int ProcessDetailStatus { get; set; }
    }
}