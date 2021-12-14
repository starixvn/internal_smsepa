namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class DepartmentFlowModel
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        public IList<ProcessFlowModel> HumanResources { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Evaluators { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Reviewers { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Managements { get; set; } = new List<ProcessFlowModel>();
    }
}