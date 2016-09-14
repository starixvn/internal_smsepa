namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public class TargetEmployeeModel
    {
        public TargetEmployeeModel()
        {
            TargetTemplates = new List<TargetTemplate>();
        }

        public bool Selected { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public Group Group { get; set; }

        public TargetTemplate RawTargetTemplate { get; set; }

        public IList<TargetTemplate> TargetTemplates { get; set; }

        public int NumberTargetTemplates
        {
            get
            {
                int numberTargetTemplates = TargetTemplates.Count;
                return numberTargetTemplates != 0 ? numberTargetTemplates : 1;
            }
        }
    }
}