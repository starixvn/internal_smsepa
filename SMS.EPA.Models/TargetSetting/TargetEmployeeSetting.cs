namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;
    using System.Linq;
    using Report;

    public class TargetEmployeeSetting : IExcelRecord
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

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Code = Employee.Code,
                Section = Employee.Group != null ? Employee.Group.GroupName : string.Empty,
                NickName = Employee.NickName,
                FullName = Employee.FullName,
                TargetType = TargetTemplates.FirstOrDefault()?.TargetTypeName,
                TargetTemplate = TargetTemplates.FirstOrDefault()?.Name
            };

            return record;
        }
    }
}