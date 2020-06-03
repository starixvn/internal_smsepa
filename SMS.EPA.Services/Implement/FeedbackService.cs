namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Feedback;
    using SMS.EPA.Model.Feedback;
    using SMS.EPA.Repositories;

    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository feedbackRepository;
        private readonly IBaseService baseService;
        private readonly IUtilityService utilityService;
        private readonly ICryptographyService cryptographyService;

        public FeedbackService() : this(null, null, null, null)
        {
        }

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            IBaseService baseService,
            IUtilityService utilityService,
            ICryptographyService cryptographyService)
        {
            this.feedbackRepository = feedbackRepository ?? new FeedbackRepository();
            this.baseService = baseService ?? new BaseService();
            this.utilityService = utilityService ?? new UtilityService();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IEnumerable<Feedback> GetByActor(int actorId, int periodId)
        {
            var criteria = new GetFeedbackByActorCriteria
            {
                ActorId = actorId,
                PeriodId = periodId
            };
            var today = DateTime.Today;
            IEnumerable<Feedback> feedbacks = feedbackRepository.GetByActor(criteria);
            foreach (Feedback feedback in feedbacks)
            {
                feedback.IsHost = DateTime.Compare(feedback.FromDate.Date, today) <= 0
                    && DateTime.Compare(feedback.ToDate.Date, today) >= 0;
                feedback.FinalScore = cryptographyService.Decrypt(feedback.FinalScore);
            }

            return feedbacks;
        }

        public Feedback GetById(long feedbackId)
        {
            var criteria = new GetFeedbackByIdCriteria { FeedbackId = feedbackId };
            Feedback feedback = feedbackRepository.GetById(criteria);
            if (feedback != null)
            {
                feedback.FinalScore = cryptographyService.Decrypt(feedback.FinalScore);
                foreach (FeedbackKPIGroup kpigroup in feedback.KPIGroups)
                {
                    double averageWeight = 0;
                    if (kpigroup.IsSpreadKPI)
                    {
                        averageWeight = kpigroup.Weight / kpigroup.KPIs.Count();
                        averageWeight = utilityService.SplitNumber(averageWeight, Constants.NumberOfDecimals);
                    }

                    foreach (FeedbackKPI kpi in kpigroup.KPIs)
                    {
                        kpi.Score = cryptographyService.Decrypt(kpi.Score);
                        kpi.Rate = cryptographyService.Decrypt(kpi.Rate);
                        kpi.Weight = kpigroup.IsSpreadKPI ? averageWeight : kpi.Weight;
                    }
                }
            }

            return feedback;
        }

        public CommandResult Update(Feedback feedback)
        {
            const string XmlTemplate = "<Row KPIGroupId='{0}' KPIId='{1}' Score='{2}' Rate='{3}' Comment='{4}' />";
            var xml = new StringBuilder(Constants.BeginRootTag);
            foreach (FeedbackKPIGroup kpiGroup in feedback.KPIGroups)
            {
                foreach (FeedbackKPI kpi in kpiGroup.KPIs)
                {
                    xml.AppendFormat(
                        XmlTemplate,
                        kpiGroup.KPIGroupId,
                        kpi.KPIId,
                        cryptographyService.Encrypt(kpi.Score),
                        cryptographyService.Encrypt(kpi.Rate),
                        HttpUtility.HtmlEncode(kpi.Comment?.Trim()));
                }
            }

            xml.Append(Constants.EndRootTag);

            var criteria = new UpdateFeedbackCriteria
            {
                FeedbackId = feedback.FeedbackId,
                FinalScore = cryptographyService.Encrypt(feedback.FinalScore),
                Comment = HttpUtility.HtmlEncode(feedback.Comment?.Trim()),
                DetailXml = xml.ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IEnumerable<FeedbackSetting> GetSetting(int periodId, int? departmentId = null, int? employeeId = null)
        {
            var criteria = new GetFeedbackSettingCriteria
            {
                PeriodId = periodId,
                DepartmentId = departmentId,
                EmployeeId = employeeId
            };
            return feedbackRepository.GetSetting(criteria);
        }

        public CommandResult UpdateSetting(UpdateFeedbackSettingCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}