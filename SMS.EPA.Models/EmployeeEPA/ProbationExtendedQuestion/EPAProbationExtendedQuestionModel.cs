namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EPAProbationExtendedQuestionModel
    {
        public string QuestionContent { get; set; }

        public IEnumerable<ProbationSubQuestionModel> SubQuestions { get; set; }
    }
}