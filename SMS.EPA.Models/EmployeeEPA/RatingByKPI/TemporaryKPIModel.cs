// -----------------------------------------------------------------------
// <copyright file="TemporaryKPIModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.09">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA.RatingByKPI
{
    using SMS.EPA.Core.Services;

    public class TemporaryKPIModel
    {
        public int EPAId { get; set; }

        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public double Weight { get; set; }

        public string ScoreEncode { get; set; }

        public string Score
        {
            get
            {
                return !string.IsNullOrEmpty(ScoreEncode) ? EncodeService.Decode(ScoreEncode).Replace("_" + KPIId, string.Empty) : string.Empty;
            }

            set
            {
                ScoreEncode = !string.IsNullOrEmpty(value) ? EncodeService.Encode(value + "_" + KPIId) : string.Empty;
            }
        }

        public string RateEncode { get; set; }

        public int Rate
        {
            get
            {
                var rate = 0;

                if (!string.IsNullOrEmpty(RateEncode))
                {
                    rate = int.Parse(EncodeService.Decode(RateEncode));
                }

                return rate;
            }
            set
            {
                RateEncode = !string.IsNullOrEmpty(value.ToString()) ? EncodeService.Encode(value.ToString()) : string.Empty;
            }
        }

        public string CommentOriginal
        {
            get
            {
                return Comment;
            }
        }

        public string Comment { get; set; }

        public string AdjustScoreEncode
        {
            get
            {
                if (!string.IsNullOrEmpty(AdjustScore))
                {
                    return EncodeService.Encode(AdjustScore + "_" + KPIId);
                }

                return string.Empty;
            }
        }

        public string AdjustScore { get; set; }

        public string AdjustRateEncode
        {
            get
            {
                if (AdjustRate > 0)
                {
                    return EncodeService.Encode(AdjustRate.ToString());
                }

                return string.Empty;
            }
        }

        public int AdjustRate { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}