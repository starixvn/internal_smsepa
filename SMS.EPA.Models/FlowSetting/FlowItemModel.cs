namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class FlowItemModel
    {
        public int Duration { get; set; }

        public bool HasSendMail { get; set; }

        public int RemindDay { get; set; }

        public bool IncludeSelfEvaluation { get; set; }

        public List<EvaluatorInfoViewModel> Employees { get; set; }
    }
}