namespace SMS.EPA.Model
{
    public class FlowTemplateModel
    {
        public FlowTemplateModel()
        {
            TargetType = new TargetTypeModel();
        }

        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public bool IsMonthly => TargetType.TargetTypeId == (int)Core.Enumerations.TargetType.Monthly;
    }
}