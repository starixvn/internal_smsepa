namespace SMS.EPA.Model
{
    using System;

    public class EPAPeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public byte TargetTypeId { get; set; }

        public string TargetTypeName { get; set; }
        public string TargetTemplate { get; set; }
    }
}