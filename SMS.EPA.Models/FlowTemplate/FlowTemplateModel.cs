namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Enitites;

    public class FlowTemplateModel
    {
        public FlowTemplateModel()
        {
            TargetType = new TargetType();
        }

        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetType TargetType { get; set; }

        public bool IsMonthly
        {
            get
            {
                return TargetType.TargetTypeId == (int)TargetTypes.Monthly;
            }
        }
    }
}