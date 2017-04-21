namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IReviewerEPAService
    {
        IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria);

        EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria);

        CommandResult AdjustScoreByReviewer(AdjustScoreByReviewerOrManagementCriteria criteria);
    }
}