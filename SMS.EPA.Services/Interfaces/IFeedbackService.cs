namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Feedback;
    using SMS.EPA.Model.Feedback;

    public interface IFeedbackService
    {
        IEnumerable<Feedback> GetByActor(int actorId, int periodId);

        Feedback GetById(long feedbackId);

        CommandResult Update(Feedback feedback);

        IEnumerable<FeedbackSetting> GetSetting(int periodId, int? departmentId = null, int? employeeId = null);

        CommandResult UpdateSetting(UpdateFeedbackSettingCriteria criteria);
    }
}