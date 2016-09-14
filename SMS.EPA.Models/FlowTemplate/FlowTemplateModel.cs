namespace SMS.EPA.Model
{
    using SMS.EPA.Model.TargetType;

    public class FlowTemplateModel
    {
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetTypeModel TargetType { get; set; }
    }
}