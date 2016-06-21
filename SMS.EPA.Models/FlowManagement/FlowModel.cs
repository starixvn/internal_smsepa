namespace SMS.EPA.Model.FlowManagement
{
    using System;
    using SMS.EPA.Model.Employee;

    [Serializable]
    public class FlowModel
    {
        public int ProcessId { get; set; }

        public int Duration { get; set; }

        public int Priority { get; set; }

        public int RemindDay { get; set; }

        public bool HasSendMail { get; set; }

        public FlowTemplateModel FlowTemplate { get; set; }

        public EmployeeModel EvaluatorInfo { get; set; }
    }
}