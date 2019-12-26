namespace SMS.EPA.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public class ProbationExtendedQuestionRepository : IProbationExtendedQuestionRepository
    {
        public EPAProbationExtendedQuestionModel GetExtendedQuestion(ICriteria criteria)
        {
            var extendedQuestion = new EPAProbationExtendedQuestionModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    extendedQuestion.QuestionContent = multi.ReadFirstOrDefault<string>();
                    extendedQuestion.SubQuestions = multi.Read<ProbationSubQuestionModel>();
                }
            }

            return extendedQuestion;
        }
    }
}
