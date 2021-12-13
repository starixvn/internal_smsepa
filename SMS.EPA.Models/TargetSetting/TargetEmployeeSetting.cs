namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;
    using System.Linq;
    using Report;

    public class TargetEmployeeSetting : IExcelRecord
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        public IList<TargetTemplateEmployee> TargetTemplates { get; set; } = new List<TargetTemplateEmployee>();

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
                No = 0,
                Code,
                Nickname,
                FullName,
                Department,
                Section,
                TargetType = TargetTemplates.FirstOrDefault()?.TargetTypeName,
                TargetTemplate = TargetTemplates.FirstOrDefault()?.Name
            };

            return record;
        }
    }
}