namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Feedback;

    public interface IFeedbackService
    {
        IEnumerable<Feedback> GetByActor(int actorId, int periodId);

        Feedback GetById(long feedbackId);

        CommandResult Update(Feedback feedback);
    }
}