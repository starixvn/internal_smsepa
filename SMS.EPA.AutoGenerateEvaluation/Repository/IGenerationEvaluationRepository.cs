// ----------------------------------------------------------------------
// <copyright file="IGenerationEvaluationRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/01/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Repository
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.AutoGenerateEvaluation.Model;
    using SMS.EPA.Model.Period;

    public interface IGenerationEvaluationRepository
    {
        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria);

        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria);
    }
}
