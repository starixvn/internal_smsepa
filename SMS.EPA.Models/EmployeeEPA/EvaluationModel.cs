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

        public string Score { get; set; }

        public string RateEncode { get; set; }

        public float Rate { get; set; }

        public string Comment { get; set; }

        public bool IsMaxScore { get; set; }
    }
}