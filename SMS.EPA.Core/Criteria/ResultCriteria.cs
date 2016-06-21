namespace SMS.EPA.Core.Criteria
{
    using Fanex.Data.Repository;

    public class ResultCriteria : CriteriaBase
    {
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}