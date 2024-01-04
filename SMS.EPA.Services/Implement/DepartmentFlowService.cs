namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;

    public class DepartmentFlowService : IDepartmentFlowService
    {
        private readonly IBaseService baseService;
        private readonly IFlowSettingRepository flowSettingRepository;

        public DepartmentFlowService() : this(null, null)
        {
        }

        public DepartmentFlowService(
            IBaseService baseService,
            IFlowSettingRepository flowSettingRepository)
        {
            this.baseService = baseService ?? new BaseService();
            this.flowSettingRepository = flowSettingRepository ?? new FlowSettingRepository();
        }

        public IEnumerable<DepartmentFlowModel> GetByFlowTemplateId(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByFlowTemplateIdCriteria
            {
                FlowTemplateId = flowTemplateId
            };
            (var flows, var processes) = baseService.FetchMultiple<DepartmentFlowModel, ProcessFlowModel>(criteria);
            MappingFlow(flows, processes);
            return flows.OrderBy(i => i.Department).ThenBy(x => x.Section);
        }

        public IEnumerable<FlowModel> GetById(GetDepartmentFlowByIdCriteria criteria)
        {
            IEnumerable<FlowModel> flow = new List<FlowModel>();
            if (criteria != null && criteria.IsValid())
            {
                flow = flowSettingRepository.GetFlowSettingByFlowTemplateId(criteria);
            }

            return flow;
        }

        private static void MappingFlow(IEnumerable<DepartmentFlowModel> flows, IEnumerable<ProcessFlowModel> processes)
        {
            foreach (DepartmentFlowModel flow in flows)
            {
                foreach (ProcessFlowModel process in processes.Where(x => x.DepartmentId == flow.DepartmentId))
                {
                    switch (process.ProcessId)
                    {
                        case (byte)ProcessType.HumanResource:
                            flow.HumanResources.Add(process);
                            break;

                        case (byte)ProcessType.Evaluator:
                            flow.Evaluators.Add(process);
                            break;

                        case (byte)ProcessType.Reviewer:
                            flow.Reviewers.Add(process);
                            break;

                        case (byte)ProcessType.Management:
                            flow.Managements.Add(process);
                            break;

                        default:
                            continue;
                    }
                }
            }
        }
    }
}