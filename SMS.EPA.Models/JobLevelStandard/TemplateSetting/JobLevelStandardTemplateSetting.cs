namespace SMS.EPA.Model.JobLevelStandard.TemplateSetting
{
    using SMS.EPA.Model.Report;

    public class JobLevelStandardTemplateSetting : IExcelRecord
    {
        public int ObjectId { get; set; }
        public byte ObjectTypeId { get; set; }
        public string UnitName { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string Template { get; set; }

        public object ConvertToExcelRecord() => new
        {
            No = 0,
            UnitName,
            DepartmentName,
            SectionName,
            Template
        };
    }
}