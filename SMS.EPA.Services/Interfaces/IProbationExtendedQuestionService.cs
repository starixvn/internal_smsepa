namespace SMS.EPA.Services.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IProbationExtendedQuestionService
    {
        EPAProbationExtendedQuestionModel GetExtendedQuestion(ICriteria criteria);
    }
}