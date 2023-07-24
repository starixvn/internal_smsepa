namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class TargetDepartmentSetting
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string Unit { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }
        public bool IsGroup { get; set; }

        public IList<TargetTemplateDepartment> TargetTemplates { get; set; } = new List<TargetTemplateDepartment>();

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