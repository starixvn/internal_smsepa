// -----------------------------------------------------------------------
// <copyright file="EvaluationModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;

    public class EvaluationModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int KPIId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

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

        public int Percent
        {
            get
            {
                return Rate * Constants.PercentPerStar;
            }
        }

        public string Comment { get; set; }
    }
}