namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IProbationExtendedQuestionRepository
    {
        EPAProbationExtendedQuestionModel GetExtendedQuestion(ICriteria criteria);
    }
}
