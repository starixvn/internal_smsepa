namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class InsertFlowItemModel
    {
        public int Duration { get; set; }

        public bool HasSendMail { get; set; }

        public int RemindDay { get; set; }

        public bool IncludeSelfEvaluation { get; set; }

        public List<Employee> Employees { get; set; }
    }
}