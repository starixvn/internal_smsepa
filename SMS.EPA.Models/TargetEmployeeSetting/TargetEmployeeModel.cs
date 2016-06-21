namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    public class TargetEmployeeModel
    {
        public TargetEmployeeModel()
        {
            TargetTemplates = new List<TargetTemplateModel>();
        }

        public bool Selected { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public Group Group { get; set; }

        public TargetTemplateModel RawTargetTemplate { get; set; }

        public IList<TargetTemplateModel> TargetTemplates { get; set; }

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