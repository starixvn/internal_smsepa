namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class PersonalFlowModel
    {
        public PersonalFlowModel()
        {
            HumanResources = new List<ProcessFlowModel>();
            Evaluators = new List<ProcessFlowModel>();
            Reviewers = new List<ProcessFlowModel>();
            Managements = new List<ProcessFlowModel>();
        }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public Group Group { get; set; }

        public IList<ProcessFlowModel> HumanResources { get; set; }

        public IList<ProcessFlowModel> Evaluators { get; set; }

        public IList<ProcessFlowModel> Reviewers { get; set; }

        public IList<ProcessFlowModel> Managements { get; set; }
    }
}