// -----------------------------------------------------------------------
// <copyright file="EvaluateModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    public class EvaluateModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int KPIId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Score { get; set; }

        public int Rate { get; set; }

        public int Percent
        {
            get
            {
                if (Rate > 0)
                {
                    return Rate * 20;
                }

                return 0;
            }
        }

        public string Comment { get; set; }
    }
}