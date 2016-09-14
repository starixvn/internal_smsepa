namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public class TargetDepartmentModel : Department
    {
        public TargetDepartmentModel()
        {
            TargetTemplates = new List<TargetTemplate>();
        }

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