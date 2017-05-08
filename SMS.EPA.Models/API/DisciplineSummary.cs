namespace SMS.EPA.Model.API
{
    using Model.Discipline;
    using System;

    public class DisciplineSummary : DisciplineSummaryBase
    {
        public int EmployeeId { get; set; }

        public DateTime Period { get; set; }
    }
}
