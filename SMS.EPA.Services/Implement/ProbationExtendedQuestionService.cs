namespace SMS.EPA.Services.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class ProbationExtendedQuestionService : IProbationExtendedQuestionService
    {
        private readonly IBaseService baseService;

        public ProbationExtendedQuestionService() : this(null)
        {
        }

        public ProbationExtendedQuestionService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public EPAProbationExtendedQuestionModel GetExtendedQuestion(ICriteria criteria)
        {
            var extendedQuestion = new EPAProbationExtendedQuestionModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    extendedQuestion.QuestionContent = multi.ReadSingle<string>();
                    extendedQuestion.SubQuestions = multi.Read<ProbationSubQuestionModel>();
                }
            }

            return extendedQuestion;
        }
    }
}