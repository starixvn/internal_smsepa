namespace SMS.EPA.AutoGenerateEvaluation.Repository
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IEvaluationRepository
    {
        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria);
    }
}