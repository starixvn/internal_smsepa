namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class ViolationModel
    {
        public int ViolationId { get; set; }

        public decimal Score { get; set; }

        public int Value { get; set; }

        public bool HasIgnored { get; set; }
    }
}