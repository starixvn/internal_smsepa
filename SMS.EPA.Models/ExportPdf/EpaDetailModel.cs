namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EpaDetailModel
    {
        public EPAModel EpaInfo { get; set; }

        public IEnumerable<StandardGradeModel> Grades { get; set; }
    }
}