namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.TargetTemplate;

    public class TargetDepartmentModel : DepartmentModel
    {
        public TargetDepartmentModel()
        {
            TargetTemplates = new List<TargetTemplateModel>();
        }

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