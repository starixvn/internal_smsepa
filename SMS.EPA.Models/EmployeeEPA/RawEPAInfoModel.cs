// -----------------------------------------------------------------------
// <copyright file="RawEPAInfoModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.03">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;

    public class RawEPAInfoModel
    {
        public EPAModel EPA { get; set; }

        public IEnumerable<EPAProcessModel> Processes { get; set; }

        public IEnumerable<EPAKpiModel> KPIs { get; set; }

        public IEnumerable<EvaluationModel> Evaluations { get; set; }

        public IEnumerable<EvaluatorModel> Evaluators { get; set; }
    }
}