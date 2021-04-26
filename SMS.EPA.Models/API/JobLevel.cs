namespace SMS.EPA.Model.API
{
    using System;

    public class JobLevel
    {
        public int JobLevelId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}