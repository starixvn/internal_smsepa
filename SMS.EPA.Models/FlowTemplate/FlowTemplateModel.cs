namespace SMS.EPA.Model
{
    using SMS.EPA.Model.Enitites;

    public class FlowTemplateModel
    {
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetType TargetType { get; set; }
    }
}