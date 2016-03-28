// -----------------------------------------------------------------------
// <copyright file="EvaluationProcessModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.12">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;

    public class EvaluationProcessModel
    {
        public EvaluationProcessModel()
        {
            HumanResources = new List<EPAProcessModel>();
            Evaluators = new List<EPAProcessModel>();
            Reviewers = new List<EPAProcessModel>();
            Managements = new List<EPAProcessModel>();
        }

        public IList<EPAProcessModel> HumanResources { get; set; }

        public EPAProcessModel Self { get; set; }

        public IList<EPAProcessModel> Evaluators { get; set; }

        public IList<EPAProcessModel> Reviewers { get; set; }

        public IList<EPAProcessModel> Managements { get; set; }
    }
}