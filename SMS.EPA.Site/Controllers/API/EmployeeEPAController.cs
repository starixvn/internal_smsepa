// -----------------------------------------------------------------------
// <copyright file="EmployeeEPAController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.10">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly CommonController _commonController;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService employeeEPAService,
            CommonController commonController)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _commonController = commonController ?? new CommonController();
        }

        #region Self

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEmployee(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeInfo.EmployeeId, (int)ProcessType.Self);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEmployeeCriteria()
                    {
                        EPAId = epaId,
                        EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByEmployee(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = (int)ProcessType.Self,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.CurrentProcess.Comment,
                EPAProcess = model.CurrentProcess,
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return UpdateEPAScoreByEmployeeOrEvaluator(criteria);
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
            bool? isLocal = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Evaluator, groupId, isLocal);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByEvaluator(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeInfo.EmployeeId, (int)ProcessType.Evaluator);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByEvaluatorCriteria()
                    {
                        EPAId = epaId,
                        EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByEvaluator(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEvaluator(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = (int)ProcessType.Evaluator,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.CurrentProcess.Comment,
                EPAProcess = model.CurrentProcess,
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return UpdateEPAScoreByEmployeeOrEvaluator(criteria);
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
            bool? isLocal = null)
        {
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Reviewer, groupId, isLocal);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByReviewer(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeInfo.EmployeeId, (int)ProcessType.Reviewer);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByReviewerCriteria()
                    {
                        EPAId = epaId,
                        ReviewerId = CurrentUser.EmployeeInfo.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByReviewer(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Host)]
        public CommandResult AdjustScoreByReviewer(IEnumerable<EPAModel> memberEPAs)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Reviewer,
                MemberEPAs = memberEPAs,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
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
            return GetMemberByEvaluatorOrReviewerOrManagement((int)ProcessType.Management, groupId, isLocal);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            bool? isLocal = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByManagementCriteria()
            {
                ManagementId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                IsLocal = isLocal,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public HttpResponseMessage GetEPAInfoByManagement(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeInfo.EmployeeId, (int)ProcessType.Management);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByManagementCriteria()
                    {
                        EPAId = epaId,
                        ManagementId = CurrentUser.EmployeeInfo.EmployeeId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoByManagement(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Host)]
        public CommandResult AdjustScoreByManagement(IEnumerable<EPAModel> memberEPAs)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Management,
                MemberEPAs = memberEPAs,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        #endregion Management

        #region Rating By KPI

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator()
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetKPIByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(
            int targetTemplateId,
            int kpiId,
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEmployeeEPAByKPICriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEmployeeEPAByKPI(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Host)]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var criteria = new UpdateKPIScoreCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EpaInfos = epaInfos
            };

            return _employeeEPAService.UpdateKPIScore(criteria);
        }

        #endregion Rating By KPI

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            EPAModel epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId, CurrentUser.EmployeeInfo.EmployeeId);

            switch (permission)
            {
                case (int)Policy.None:
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);

                default:
                    var criteria = new GetEPAInfoByIdCriteria()
                    {
                        EPAId = epaId
                    };

                    epaInfo = _employeeEPAService.GetEPAInfoById(criteria);

                    return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
            }
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            var getEPASummaryImportCriteria = new GetEPASummaryImportCriteria();
            IEnumerable<EPASummaryImportModel> memberEPAs = _employeeEPAService.GetMemberEPASummaryImport(getEPASummaryImportCriteria);
            var criteria = new UpdateEPASummaryImportCriteria()
            {
                MemberEPAs = memberEPAs
            };

            CommandResult result = _employeeEPAService.UpdateEPASummaryImport(criteria);
            return result;
        }

        private CommandResult UpdateEPAScoreByEmployeeOrEvaluator(UpdateEPAScoreCriteria criteria)
        {
            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        private IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(
            int processId,
            int? groupId = null,
            bool? isLocal = null)
        {
            var criteria = new GetMemberByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = processId,
                GroupId = groupId,
                IsLocal = isLocal
            };

            return _employeeEPAService.GetMemberByEvaluatorOrReviewerOrManagement(criteria);
        }

        private IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(int processId)
        {
            var criteria = new GetGroupByEvaluatorOrReviewerOrManagementCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = processId
            };

            return _employeeEPAService.GetGroupByEvaluatorOrReviewerOrManagement(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return _employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
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

            return _employeeEPAService.CheckEmployeePermission(checkPermissionCriteria);
        }
    }
}