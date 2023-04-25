namespace SMS.EPA.ServicesAPI.Criteria.Todolist
{
    using Fanex.Data.Repository;

    public class GetTodolistJobLevelStandardByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "API_GO_GetTodolistJobLevelStandardByUserId";

        public override bool IsValid() => UserId > 0;
    }
}