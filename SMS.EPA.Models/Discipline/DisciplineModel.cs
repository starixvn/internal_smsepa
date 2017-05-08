namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class DisciplineModel
    {
        public int DisciplineId { get; set; }

        public decimal Score { get; set; }

        public int Value { get; set; }

        public bool HasIgnored { get; set; }
    }
}