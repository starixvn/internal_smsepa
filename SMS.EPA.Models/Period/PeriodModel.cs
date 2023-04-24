namespace SMS.EPA.Model
{
    using System;

    public class PeriodModel
    {
        public PeriodModel()
        {
            TargetType = new TargetTypeModel();
        }

        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? AttendanceStartDate { get; set; }

        public DateTime? AttendanceEndDate { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalEmployeesUnfinished { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public int? Year
        {
            get
            {
                int? year = null;

                if (StartDate != null)
                {
                    year = StartDate.Value.Year;
                }

                return year;
            }
        }

        public bool IsSemiAnnual => TargetType.TargetTypeId == (byte)Core.Enumerations.TargetType.SemiAnnual;

        public bool IsProbation => TargetType.TargetTypeId == (byte)Core.Enumerations.TargetType.Probation;

        public bool IsPromotion => TargetType.TargetTypeId == (byte)Core.Enumerations.TargetType.Promotion;
    }
}