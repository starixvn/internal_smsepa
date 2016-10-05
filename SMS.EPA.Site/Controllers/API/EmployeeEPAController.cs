namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICryptographyService cryptographyService;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(IEmployeeEPAService employeeEPAService, ICryptographyService cryptographyService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        #region Self

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = this.SessionService.EmployeeId
            };

            return employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, this.SessionService.EmployeeId, (int)ProcessType.Self);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEmployeeCriteria()
                    {
                        EPAId = epaId,
                        EmployeeId = this.SessionService.EmployeeId
                    };

                    epaInfo = employeeEPAService.GetEPAInfoByEmployee(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Self;
                criteria.EvaluatorId = this.SessionService.EmployeeId;
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEmployee()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Self);
            return permission;
        }

        #endregion Self

        #region Evaluator

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByEvaluator()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Evaluator);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Evaluator, groupId, isLocal, isMonthly);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId, int? evaluatorId = null)
        {
            var epaInfo = new EPAModel();
            var processId = evaluatorId == null ? (int?)ProcessType.Evaluator : null;
            evaluatorId = evaluatorId ?? this.SessionService.EmployeeId;

            int permission = CheckEmployeePermission(epaId, evaluatorId.Value, processId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEvaluatorCriteria()
                    {
                        EPAId = epaId,
                        EvaluatorId = evaluatorId.Value
                    };

                    epaInfo = employeeEPAService.GetEPAInfoByEvaluator(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ProcessId = (int)ProcessType.Evaluator;
                criteria.EvaluatorId = this.SessionService.EmployeeId;
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = UpdateEPAScoreByEmployeeOrEvaluator(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnEvaluator()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Evaluator);
            return permission;
        }

        #endregion Evaluator

        #region Reviewer

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByReviewer()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Reviewer);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Reviewer, groupId, isLocal, isMonthly);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            bool isMonthly = false,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = this.SessionService.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return employeeEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, this.SessionService.EmployeeId, (int)ProcessType.Reviewer);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria()
                    {
                        EPAId = epaId,
                        ReviewerId = this.SessionService.EmployeeId
                    };

                    epaInfo = employeeEPAService.GetEPAInfoByReviewer(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Host)]
        public CommandResult AdjustScoreByReviewer(IEnumerable<AdjustScoreViewModel> epaInfos)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Reviewer,
                EpaInfos = epaInfos,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        public int GetUserPermissionOnReviewer()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Reviewer);
            return permission;
        }

        #endregion Reviewer

        #region Management

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Group> GetGroupByManagement()
        {
            return GetGroupByEvaluatorOrReviewerOrManagement((int)ProcessType.Management);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByManagement(
            int? groupId = null,
            bool? isLocal = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Management, groupId, isLocal, false);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null,
            bool? statusProcess = null)
        {
            var criteria = new GetEPAByManagementCriteria()
            {
                ManagementId = this.SessionService.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds,
                StatusProcess = statusProcess
            };

            return employeeEPAService.GetEPAByManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByManagement(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, this.SessionService.EmployeeId, (int)ProcessType.Management);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByManagementCriteria()
                    {
                        EPAId = epaId,
                        ManagementId = this.SessionService.EmployeeId
                    };

                    epaInfo = employeeEPAService.GetEPAInfoByManagement(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Host)]
        public CommandResult AdjustScoreByManagement(IEnumerable<AdjustScoreViewModel> epaInfos)
        {
            CommandResult commandResult = new CommandResult();

            if (epaInfos != null && epaInfos.Any())
            {
                var criteria = new AdjustScoreByReviewerOrManagementCriteria()
                {
                    ProcessId = (int)ProcessType.Management,
                    EpaInfos = epaInfos,
                    ModifiedBy = this.SessionService.EmployeeId
                };

                commandResult = AdjustScoreByReviewerOrManagement(criteria);
            }

            return commandResult;
        }

        public int GetUserPermissionOnManagement()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.Management);
            return permission;
        }

        #endregion Management

        #region Rating By KPI

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByEvaluatorCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId
            };

            return employeeEPAService.GetPeriodByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(int periodId)
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId
            };

            return employeeEPAService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodToView()
        {
            var criteria = new GetPeriodToViewCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId
            };

            return employeeEPAService.GetPeriodToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateToView(int periodId)
        {
            var criteria = new GetTargetTemplateToViewCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId
            };

            return employeeEPAService.GetTargetTemplateToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetEvaluatorsByTargetTemplateCriteria()
            {
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return employeeEPAService.GetEvaluatorsByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIToView(
            int periodId,
            int targetTemplateId,
            int evaluatorId)
        {
            var criteria = new GetKPIToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return employeeEPAService.GetKPIToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int evaluatorId)
        {
            var criteria = new GetEmployeeEPAToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId
            };

            return employeeEPAService.GetEmployeeEPAToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                TargetTemplateId = targetTemplateId,
                PeriodId = periodId
            };

            return employeeEPAService.GetKPIByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEmployeeEPAByKPICriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return employeeEPAService.GetEmployeeEPAByKPI(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Host)]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var criteria = new UpdateKPIScoreCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                EpaInfos = epaInfos
            };

            return employeeEPAService.UpdateKPIScore(criteria);
        }

        public int GetUserPermissionOnRatingByKPI()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.RatingByKPI);
            return permission;
        }

        #endregion Rating By KPI

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, this.SessionService.EmployeeId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    epaInfo = employeeEPAService.GetEPAInfoById(epaId);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            var getEPASummaryImportCriteria = new GetEPASummaryImportCriteria();
            IEnumerable<EPASummaryImportModel> memberEPAs = employeeEPAService.GetMemberEPASummaryImport(getEPASummaryImportCriteria);
            var criteria = new UpdateEPASummaryImportCriteria()
            {
                MemberEPAs = memberEPAs
            };

            CommandResult result = employeeEPAService.UpdateEPASummaryImport(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(int epaId)
        {
            HttpResponseMessage response = null;

            if (this.SessionService.CurrentUser.Roles.Select(x => x.RoleId).Contains((int)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = employeeEPAService.GetEPAInfoById(epaId);

                var resetScoreCriteria = new ResetScoreCriteria()
                {
                    EPAId = epaInfo.EPAId,
                    ModifiedBy = this.SessionService.EmployeeId
                };

                CommandResult commandResult;
                if (epaInfo.HasMonthlyEPA
                    && epaInfo.Period.TargetTypeId == (int)TargetTypes.SemiAnnual)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        resetScoreCriteria.EPAInfo = epaInfo;

                        commandResult = employeeEPAService.ResetScore(resetScoreCriteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    resetScoreCriteria.EPAInfo = epaInfo;

                    commandResult = employeeEPAService.ResetScore(resetScoreCriteria);
                    response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            return response;
        }

        #region Update Final Score

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetPeriodToUpdateFinalScore()
        {
            var criteria = new GetPeriodToUpdateFinalScoreCriteria();
            IList<EPAPeriodModel> periods = employeeEPAService.GetPeriodToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, periods);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEmployeeToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            bool? isLocal = null,
            string query = "")
        {
            var criteria = new GetEmployeeToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                Query = query
            };
            IList<EPAEmployeeModel> employees = employeeEPAService.GetEmployeeToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, employees);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAToUpdateFinalScore(
            int periodId,
            int? groupId = null,
            bool? isLocal = null,
            int? employeeId = null)
        {
            var criteria = new GetEPAToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeId = employeeId
            };
            IList<EPAModel> epaInfos = employeeEPAService.GetEPAToUpdateFinalScore(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UpdateFinalScore, Policy = Policy.Host)]
        public HttpResponseMessage UpdateFinalScore(IList<EPAModel> selectedEPAs)
        {
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();

            foreach (EPAModel epaInfo in selectedEPAs)
            {
                float averagePerformanceScore = GetFinalScoreOfEPA(epaInfo),
                    finalScore = averagePerformanceScore + epaInfo.DisciplineScore;

                finalScore = finalScore >= 0 ? finalScore : 0;
                epaInfo.FinalScore = cryptographyService.Encrypt(finalScore.ToString());
                string grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                epaInfo.Grade = cryptographyService.Encrypt(grade);
            }

            var updateFinalScoreCriteria = new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = this.SessionService.EmployeeId,
                EPAInfos = selectedEPAs
            };

            CommandResult commandResult = employeeEPAService.UpdateFinalScoreEPAEndEvaluation(updateFinalScoreCriteria);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, commandResult);

            return response;
        }

        #endregion Update Final Score

        private CommandResult UpdateEPAScoreByEmployeeOrEvaluator(UpdateEPAScoreCriteria criteria)
        {
            return employeeEPAService.UpdateEPAScore(criteria);
        }

        private IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(
            int processId,
            int? groupId = null,
            bool? isLocal = null,
            bool? isMonthly = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = this.SessionService.EmployeeId,
                ProcessId = processId,
                GroupId = groupId,
                IsLocal = isLocal,
                IsMonthly = isMonthly
            };

            return employeeEPAService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        private IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(int processId)
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = this.SessionService.EmployeeId,
                ProcessId = processId
            };

            return employeeEPAService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
        }

        private int CheckEmployeePermission(
            int epaId,
            int actorUserId,
            int? processId = null)
        {
            var checkPermissionCriteria = new GetEmployeePermissionCriteria()
            {
                EPAId = epaId,
                ActorUserId = actorUserId,
                ProcessId = processId
            };

            return employeeEPAService.CheckEmployeePermission(checkPermissionCriteria);
        }

        private float GetFinalScoreOfEPA(EPAModel epaInfo)
        {
            float finalScore = 0;

            if (epaInfo.Process.Managements.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByManagements;
            }
            else if (epaInfo.Process.Reviewers.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByReviewers;
            }

            return finalScore;
        }
    }
}