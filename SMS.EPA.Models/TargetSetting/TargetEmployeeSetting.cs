namespace SMS.EPA.Model
{
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;

    public class TargetEmployeeSetting
    {
        public TargetEmployeeSetting()
        {
            TargetTemplates = new List<TargetTemplateEmployee>();
        }

        public Employee Employee { get; set; }

        public IList<TargetTemplateEmployee> TargetTemplates { get; set; }

        public int NumberTargetTemplates
        {
            get
            {
                int numberTargetTemplates = TargetTemplates.Count;
                return numberTargetTemplates != 0 ? numberTargetTemplates : 1;
            }
        }

        public bool Selected { get; set; }
    }
}