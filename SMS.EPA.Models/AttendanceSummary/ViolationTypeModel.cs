namespace SMS.EPA.Model
{
    public class ViolationTypeModel
    {
        public int ViolationId { get; set; }

        public string Name { get; set; }

        public float Value { get; set; }

        public bool IsActive { get; set; }
    }
}