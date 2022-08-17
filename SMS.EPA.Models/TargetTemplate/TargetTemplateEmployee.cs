namespace SMS.EPA.Model
{
    public class TargetTemplateEmployee
    {
        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public int EmployeeId { get; set; }

        public byte TargetTypeId { get; set; }

        public string TargetTypeName { get; set; }
    }
}