namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class FlowModel
    {
        public byte ProcessId { get; set; }

        public int Duration { get; set; }

        public int Priority { get; set; }

        public int RemindDay { get; set; }

        public bool HasSendMail { get; set; }

        public FlowTemplateModel FlowTemplate { get; set; } = new FlowTemplateModel();

        public EvaluatorInfoViewModel EvaluatorInfo { get; set; } = new EvaluatorInfoViewModel();
    }
}