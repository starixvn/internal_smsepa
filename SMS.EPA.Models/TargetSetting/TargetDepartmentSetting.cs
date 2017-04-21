namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;

    public class TargetDepartmentSetting
    {
        public TargetDepartmentSetting()
        {
            TargetTemplates = new List<TargetTemplateDepartment>();
        }

        public Department Department { get; set; }

        public IList<TargetTemplateDepartment> TargetTemplates { get; set; }

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