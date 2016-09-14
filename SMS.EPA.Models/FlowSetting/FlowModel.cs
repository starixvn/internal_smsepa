namespace SMS.EPA.Model
{
    using System;
    using SMS.EPA.Model.Employee;

    [Serializable]
    public class FlowModel
    {
        public FlowModel()
        {
            EvaluatorInfo = new EmployeeModel();
            FlowTemplate = new FlowTemplateModel();
        }

        public int ProcessId { get; set; }

        public int Duration { get; set; }

        public int Priority { get; set; }

        public int RemindDay { get; set; }

        public bool HasSendMail { get; set; }

        public FlowTemplateModel FlowTemplate { get; set; }

        public EmployeeModel EvaluatorInfo { get; set; }
    }
}