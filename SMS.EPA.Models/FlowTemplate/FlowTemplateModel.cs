namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;

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

        public bool IsMonthly
        {
            get
            {
                return TargetType.TargetTypeId == (int)SMS.EPA.Core.Enumerations.TargetType.Monthly;
            }
        }
    }
}