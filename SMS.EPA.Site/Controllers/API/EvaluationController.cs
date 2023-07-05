namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services;

    public class EvaluationController : BaseController
    {
        private readonly IRoleService roleService;
        private readonly IEmployeeEPAService evaluationService;
        private readonly IBaseService baseService;

        public EvaluationController()
            : this(null, null, null)
        {
        }

        public EvaluationController(
            IRoleService roleService,
            IEmployeeEPAService evaluationService,
            IBaseService baseService)
        {
            this.roleService = roleService ?? new RoleService();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        public HttpResponseMessage GetEPAInfoById(int epaId)
        {
            var epaInfo = new EPAModel();
            int permission = CheckEmployeePermission(epaId);

            if (permission == (byte)Policy.None)
            {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, epaInfo);
            }

            epaInfo = evaluationService.GetEPAInfoById(epaId);

            IList<Role> roles = roleService.GetBySiteId(UserControlSession.SiteId);
            evaluationService.RemoveSensitiveDataOnEmployeeRole(epaInfo, CurrentEmployeeId, roles);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfo);
        }

        [HttpPut]
        public CommandResult ConvertEPA()
        {
            IEnumerable<EPASummaryImportModel> epaInfos = baseService.GetData<EPASummaryImportModel>(new GetEPASummaryImportCriteria());
            var criteria = new UpdateEPASummaryImportCriteria
            {
                EpaInfos = epaInfos
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return result;
        }

        [HttpPut]
        public HttpResponseMessage ResetScore(ResetScoreCriteria criteria)
        {
            HttpResponseMessage response = null;

            if (roleService.GetByUserId(UserControlSession.UserId).Select(x => x.RoleId).Contains((byte)SystemRole.SystemAdmin))
            {
                EPAModel epaInfo = evaluationService.GetEPAInfoById(criteria.EPAId);

                criteria.ModifiedBy = CurrentEmployeeId;

                CommandResult commandResult;
                if (epaInfo.IsSemiHasMonthly)
                {
                    if (epaInfo.Process.Reviewers.Any()
                        && epaInfo.Process.Reviewers.First().StartDate.Date <= DateTime.Now.Date)
                    {
                        criteria.EPAInfo = epaInfo;

                        commandResult = baseService.ExecuteNonQuery(criteria);
                        response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                    }
                }
                else
                {
                    criteria.EPAInfo = epaInfo;

                    commandResult = baseService.ExecuteNonQuery(criteria);
                    response = Request.CreateResponse(HttpStatusCode.OK, commandResult);
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }

            return response;
        }

        private byte CheckEmployeePermission(int epaId)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = epaId,
                ActorUserId = CurrentEmployeeId
            };

            return baseService.GetDataEntity<byte>(criteria);
        }
    }
}