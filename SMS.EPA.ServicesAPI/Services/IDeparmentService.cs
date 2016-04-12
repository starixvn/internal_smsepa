namespace SMS.EPA.ServicesAPI.Services
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.ServicesAPI.Criteria;

    public interface IDeparmentService
    {
        CommandResult Update(InsertAndUpdateDepartmentCriteria criteria);
    }
}
