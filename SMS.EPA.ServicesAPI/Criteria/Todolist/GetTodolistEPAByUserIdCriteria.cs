namespace SMS.EPA.ServicesAPI.Criteria.Todolist
{
    using Fanex.Data.Repository;

    public class GetTodolistEPAByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "API_GO_GetTodolistEPAByUserId";

        public override bool IsValid() => UserId > 0;
    }
}