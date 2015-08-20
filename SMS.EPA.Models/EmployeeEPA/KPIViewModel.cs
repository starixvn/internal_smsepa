// -----------------------------------------------------------------------
// <copyright file="KPIViewModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.20">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    public class KPIViewModel
    {
        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public double? Weight { get; set; }

        public string ScoreBySelf { get; set; }

        public int RateBySelf { get; set; }

        public int PercentBySelf
        {
            get
            {
                if (RateBySelf > 0)
                {
                    return RateBySelf * 20;
                }

                return 0;
            }
        }

        public string CommentBySelf { get; set; }

        public string ScoreByEvaluator { get; set; }

        public int RateByEvaluator { get; set; }

        public int PercentByEvaluator
        {
            get
            {
                if (RateByEvaluator > 0)
                {
                    return RateByEvaluator * 20;
                }

                return 0;
            }
        }

        public string CommentByEvaluator { get; set; }
    }
}