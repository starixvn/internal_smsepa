namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Feedback;

    public interface IFeedbackRepository
    {
        IEnumerable<Feedback> GetByActor(ICriteria criteria);

        Feedback GetById(ICriteria criteria);
    }
}